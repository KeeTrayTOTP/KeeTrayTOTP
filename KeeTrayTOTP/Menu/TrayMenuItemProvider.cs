using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Utility;
using KeeTrayTOTP.Helpers;
using KeeTrayTOTP.Localization;
using KeeTrayTOTP.Properties;

namespace KeeTrayTOTP.Menu
{
    public class TrayMenuItemProvider : MenuItemProviderBase
    {
        protected readonly KeeTrayTOTPExt Plugin;
        protected readonly IPluginHost PluginHost;
        protected readonly DocumentManagerEx DocumentManager;

        private bool _trimTrayMenuTextEnabled;

        private ToolStripMenuItem _rootTrayMenuItem;

        public TrayMenuItemProvider(KeeTrayTOTPExt plugin, IPluginHost pluginHost)
        {
            Plugin = plugin;
            DocumentManager = pluginHost.MainWindow.DocumentManager;
            PluginHost = pluginHost;
        }

        public override ToolStripMenuItem ProvideMenuItem()
        {
            _rootTrayMenuItem = new ToolStripMenuItem(Strings.TrayTOTPPlugin, Resources.TOTP);

            _rootTrayMenuItem.DropDownItems.Add(CreatePseudoToolStripMenuItem());

            _rootTrayMenuItem.DropDownOpening += OnRootDropDownOpening;
            _rootTrayMenuItem.DropDownClosed += OnDropDownClosed;

            return _rootTrayMenuItem;
        }

        /// <summary>
        ///     This menu item is required to show the dropdown arrow in the tray context menu,
        ///     even if the menu is still empty. (because we don't fill it until the opening event)
        /// </summary>
        private static ToolStripMenuItem CreatePseudoToolStripMenuItem()
        {
            return new ToolStripMenuItem();
        }

        private void OnRootDropDownOpening(object sender, EventArgs e)
        {
            var rootTrayMenuItem = sender as ToolStripMenuItem;
            if (rootTrayMenuItem == null)
            {
                return;
            }

            rootTrayMenuItem.DropDownItems.Clear();

            var documents = PluginHost.MainWindow.DocumentManager.Documents;
            if (DocumentManager.IsNotAtLeastOneDocumentOpen())
            {
                var noDatabaseOpenedMenuItem = new ToolStripMenuItem(Strings.NoDatabaseIsOpened, Resources.TOTP_Error);
                rootTrayMenuItem.DropDownItems.Add(noDatabaseOpenedMenuItem);
                return;
            }

            _trimTrayMenuTextEnabled = PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);

            IEnumerable<ToolStripMenuItem> menuItems = null;
            if (DocumentManager.IsSingleDatabaseOpenAndUnlocked()) 
            {
                // show entries directly as dropdown of the root menu
                menuItems = CreateMenuItemsFromPwGroup(documents[0].Database.RootGroup);
            }
            else 
            {
                // show entries for each opened (but potential locked) database
                menuItems = documents.Select(CreateMenuItemForDocument);
            }
            rootTrayMenuItem.DropDownItems.AddRange(menuItems.Cast<ToolStripItem>().ToArray());
            
        }

        private ToolStripMenuItem CreateMenuItemForDocument(PwDocument document)
        {
            ToolStripMenuItem mainDropDownItem;
            if (!document.Database.IsOpen)
            {
                var documentName = UrlUtil.GetFileName(document.LockedIoc.Path);
                EventHandler t = (o, e) => PluginHost.MainWindow.OpenDatabase(document.LockedIoc, null, true);
                documentName += " [" + Strings.Locked + "]";
                mainDropDownItem = new ToolStripMenuItem(documentName, Resources.TOTP_Error, t);
            }
            else
            {
                var documentName = UrlUtil.GetFileName(document.Database.IOConnectionInfo.Path);
                mainDropDownItem = new ToolStripMenuItem(documentName, ImageHelper.CreateImageFromColor(document.Database.Color));
                mainDropDownItem.Tag = document;
                mainDropDownItem.DropDownOpening += OnDatabaseDropDownOpening;
                mainDropDownItem.DropDownOpening += MenuItemHelper.OnDatabaseDropDownOpening;
                mainDropDownItem.DropDownClosed += OnDropDownClosed;
                mainDropDownItem.DropDownItems.Add(CreatePseudoToolStripMenuItem());
            }

            return mainDropDownItem;
        }

        private void OnDatabaseDropDownOpening(object sender, EventArgs e)
        {
            var databaseMenuItem = sender as ToolStripMenuItem;
            if (databaseMenuItem == null)
            {
                return;
            }

            var pwDocument = databaseMenuItem.Tag as PwDocument;
            if (pwDocument == null)
            {
                return;
            }

            databaseMenuItem.DropDownItems.Clear();

            var menuItems = CreateMenuItemsFromPwGroup(pwDocument.Database.RootGroup);
            databaseMenuItem.DropDownItems.AddRange(menuItems.Cast<ToolStripItem>().ToArray());
        }

        private IEnumerable<ToolStripMenuItem> CreateMenuItemsFromPwGroup(PwGroup pwGroup)
        {
            return Plugin.GetVisibleAndValidPasswordEntries(pwGroup).Select(CreateMenuItemFromPwEntry);
        }

        private void OnDropDownClosed(object sender, EventArgs e)
        {
            var rootMenuItem = sender as ToolStripMenuItem;
            if (rootMenuItem == null)
            {
                return;
            }

            rootMenuItem.DropDownItems.Clear();
            rootMenuItem.DropDownItems.Add(new ToolStripMenuItem());
        }

        protected ToolStripMenuItem CreateMenuItemFromPwEntry(PwEntry entry)
        {
            var context = new SprContext(entry, PluginHost.MainWindow.ActiveDatabase, SprCompileFlags.All, false,
                false);

            var entryTitle = entry.Strings.ReadSafe(PwDefs.TitleField);
            var entryUsername = SprEngine.Compile(entry.Strings.ReadSafe(PwDefs.UserNameField), context);

            string trayTitle = TrimMenuItemTitleIfNecessary(entryTitle, entryUsername);

            var menuItem = new ToolStripMenuItem(trayTitle, Resources.TOTP_Key, OnNotifyMenuTOTPClick);
            menuItem.Tag = entry;
            if (!Plugin.SettingsValidate(entry))
            {
                menuItem.Enabled = false;
                menuItem.Image = Resources.TOTP_Error;
            }

            return menuItem;
        }

        private string TrimMenuItemTitleIfNecessary(string entryTitle, string entryUsername)
        {
            if (string.IsNullOrEmpty(entryUsername) ||
                (_trimTrayMenuTextEnabled &&
                 entryTitle.Length + entryUsername.Length > KeeTrayTOTPExt.setstat_trim_text_length))
            {
                return entryTitle.ExtWithSpaceAfter();
            }

            return entryTitle.ExtWithSpaceAfter() + entryUsername.ExtWithParenthesis();
        }

        protected void OnNotifyMenuTOTPClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = sender as ToolStripMenuItem;
            if (tsi == null)
            {
                return;
            }

            PwEntry pe = tsi.Tag as PwEntry;
            if (pe != null)
            {
                Plugin.TOTPCopyToClipboard(pe);
            }
        }

        public override void Dispose()
        {
            if (_rootTrayMenuItem != null)
            {
                _rootTrayMenuItem.DropDownOpening -= OnRootDropDownOpening;
                _rootTrayMenuItem.DropDownClosed -= OnDropDownClosed;
            }
        }
    }
}