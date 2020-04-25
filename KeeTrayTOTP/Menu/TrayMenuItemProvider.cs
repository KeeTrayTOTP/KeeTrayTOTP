using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KeePass.Forms;
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
            _rootTrayMenuItem.DropDownOpening += MenuItemExtensions.OnDatabaseDropDownOpening;
            _rootTrayMenuItem.DropDownClosed += OnDropDownClosed;

            return _rootTrayMenuItem;
        }

        private void OnRootDropDownOpening(object sender, EventArgs e)
        {
            var rootTrayMenuItem = sender as ToolStripMenuItem;
            if (rootTrayMenuItem == null)
            {
                return;
            }

            rootTrayMenuItem.DropDownItems.Clear();
            _trimTrayMenuTextEnabled = PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);

            var documents = PluginHost.MainWindow.DocumentManager.Documents;

            IEnumerable<ToolStripMenuItem> menuItems = BuildMenuItemsForRootDropDown(documents);
            rootTrayMenuItem.DropDownItems.AddRange(menuItems.Cast<ToolStripItem>().ToArray());
        }

        internal IEnumerable<ToolStripMenuItem> BuildMenuItemsForRootDropDown(List<PwDocument> documents)
        {
            if (documents.IsNotAtLeastOneDocumentOpen())
            {
                return new []
                {
                    new ToolStripMenuItem(Strings.NoDatabaseIsOpened, Resources.TOTP_Error)
                };
            }

            if (documents.IsSingleDatabaseOpenAndUnlocked())
            {
                // create entries directly as dropdown of the root menu
                return CreateDatabaseSubMenuItemsFromPwDocument(documents[0]);
            }

            // create entries for each opened (but potential locked) database
            return documents.Select(CreateDatabaseMenuItemForDocument);
        }

        private ToolStripMenuItem CreateDatabaseMenuItemForDocument(PwDocument document)
        {
            ToolStripMenuItem mainDropDownItem;
            if (!document.Database.IsOpen)
            {
                var documentName = UrlUtil.GetFileName(document.LockedIoc.Path);
                documentName += " [" + Strings.Locked + "]";
                mainDropDownItem = new ToolStripMenuItem(documentName, Resources.TOTP_Error, OnClickOpenDatabase);
            }
            else
            {
                var documentName = UrlUtil.GetFileName(document.Database.IOConnectionInfo.Path);
                mainDropDownItem = new ToolStripMenuItem(documentName, ImageExtensions.CreateImageFromColor(document.Database.Color));
                mainDropDownItem.DropDownOpening += OnDatabaseDropDownOpening;
                mainDropDownItem.DropDownOpening += MenuItemExtensions.OnDatabaseDropDownOpening;
                mainDropDownItem.DropDownClosed += OnDropDownClosed;
                mainDropDownItem.DropDownItems.Add(CreatePseudoToolStripMenuItem());
            }
            
            mainDropDownItem.Tag = document;
            return mainDropDownItem;
        }

        private void OnClickOpenDatabase(object sender, EventArgs eventArgs)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
            {
                return;
            }

            var document = menuItem.Tag as PwDocument;
            if (document != null)
            {
                PluginHost.MainWindow.OpenDatabase(document.LockedIoc, null, true);
            }
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

            var menuItems = CreateDatabaseSubMenuItemsFromPwDocument(pwDocument);
            databaseMenuItem.DropDownItems.AddRange(menuItems.Cast<ToolStripItem>().ToArray());
        }

        protected IEnumerable<ToolStripMenuItem> CreateDatabaseSubMenuItemsFromPwDocument(PwDocument pwDocument)
        {
            var validPwEntries = Plugin.GetVisibleAndValidPasswordEntries(pwDocument.Database.RootGroup).ToArray();
            if (validPwEntries.Length > 0)
            {
                return validPwEntries.Select(entry => CreateMenuItemFromPwEntry(entry, pwDocument.Database));
            }

            return NoTOTPEntriesFoundMenuItem(pwDocument);
        }

        private IEnumerable<ToolStripMenuItem> NoTOTPEntriesFoundMenuItem(PwDocument pwDocument)
        {
            return new[]
            {
                new ToolStripMenuItem("[" + Localization.Strings.NoTOTPEntriesFound + "]", Resources.TOTP_Error, OnClickOpenDatabase)
                {
                    Tag = pwDocument
                }
            };
        }

        /// <summary>
        ///     This menu item is required to show the dropdown arrow in the tray context menu,
        ///     even if the menu is still empty. (because we don't fill it until the opening event)
        /// </summary>
        private static ToolStripMenuItem CreatePseudoToolStripMenuItem()
        {
            return new ToolStripMenuItem();
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

        protected ToolStripMenuItem CreateMenuItemFromPwEntry(PwEntry entry, PwDatabase pwDatabase)
        {
            var context = new SprContext(entry, pwDatabase, SprCompileFlags.All, false, 
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