using KeePass.Plugins;
using KeePass.UI;
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Utility;
using KeeTrayTOTP.Helpers;
using KeeTrayTOTP.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP.Menu
{
    public class TrayMenuItemProvider : IMenuItemProvider
    {
        protected readonly KeeTrayTOTPExt Plugin;
        protected readonly IPluginHost PluginHost;
        protected readonly DocumentManagerEx DocumentManager;

        public TrayMenuItemProvider(KeeTrayTOTPExt plugin, IPluginHost pluginHost)
        {
            Plugin = plugin;
            DocumentManager = pluginHost.MainWindow.DocumentManager;
            PluginHost = pluginHost;
            PluginHost.MainWindow.TrayContextMenu.Opened += TrayContextMenu_Opened;
        }

        private void TrayContextMenu_Opened(object sender, EventArgs e)
        {
            var contextMenuStrip = (ContextMenuStrip)sender;
            var dropDownLocationCalculator = new DropDownLocationCalculator(contextMenuStrip.Size);
            contextMenuStrip.Location = dropDownLocationCalculator.CalculateLocationForDropDown(Cursor.Position);
        }

        public virtual ToolStripMenuItem ProvideMenuItem()
        {
            if (!Plugin.Settings.NotifyContextVisible)
            {
                return null;
            }

            var rootTrayMenuItem = new ToolStripMenuItemEx(Localization.Strings.TrayTOTPPlugin, Resources.TOTP);
            rootTrayMenuItem.ForceDropDownArrow = true;
            rootTrayMenuItem.DropDownOpening += OnRootDropDownOpening;
            rootTrayMenuItem.DropDownOpening += MenuItemHelper.OnDatabaseDropDownOpening;

            return rootTrayMenuItem;
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

            var menuItems = BuildMenuItemsForRootDropDown(documents);
            rootTrayMenuItem.DropDownItems.AddRange(menuItems.Cast<ToolStripItem>().ToArray());
        }

        internal IEnumerable<ToolStripMenuItem> BuildMenuItemsForRootDropDown(List<PwDocument> documents)
        {
            if (documents.IsNotAtLeastOneDocumentOpen())
            {
                return new[]
                {
                    new ToolStripMenuItem(Localization.Strings.NoDatabaseIsOpened, Resources.TOTP_Error)
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
            ToolStripMenuItemEx mainDropDownItem;
            if (!document.Database.IsOpen)
            {
                var documentName = UrlUtil.GetFileName(document.LockedIoc.Path);
                documentName += " [" + Localization.Strings.Locked + "]";
                mainDropDownItem = new ToolStripMenuItemEx(documentName, Resources.TOTP_Error, OnClickOpenDatabase);
            }
            else
            {
                var documentName = UrlUtil.GetFileName(document.Database.IOConnectionInfo.Path);
                mainDropDownItem = new ToolStripMenuItemEx(documentName, ImageExtensions.CreateImageFromColor(document.Database.Color));
                mainDropDownItem.ForceDropDownArrow = true;
                mainDropDownItem.DropDownOpening += OnDatabaseDropDownOpening;
                mainDropDownItem.DropDownOpening += MenuItemHelper.OnDatabaseDropDownOpening;
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
            if (document == null)
            {
                return;
            }

            PluginHost.MainWindow.OpenDatabase(document.LockedIoc, null, true);
        }

        private void OnClickShowDatabase(object sender, EventArgs eventArgs)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
            {
                return;
            }

            var document = menuItem.Tag as PwDocument;
            if (document == null)
            {
                return;
            }

            if (PluginHost.MainWindow.ActiveDatabase != document.Database)
            {
                PluginHost.MainWindow.MakeDocumentActive(document);
            }
            PluginHost.MainWindow.EnsureVisibleForegroundWindow(true, true);
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
            var validPwEntries = Plugin.GetVisibleAndValidPasswordEntries(pwDocument.Database).ToArray();
            if (validPwEntries.Length > 0)
            {
                return validPwEntries
                    .Select(entry => CreateMenuItemFromPwEntry(entry, pwDocument.Database))
                    .OrderBy(t => t.Text);
            }

            return NoTOTPEntriesFoundMenuItem(pwDocument);
        }

        private IEnumerable<ToolStripMenuItem> NoTOTPEntriesFoundMenuItem(PwDocument pwDocument)
        {
            return new[]
            {
                new ToolStripMenuItem("[" + Localization.Strings.NoTOTPEntriesFound + "]", Resources.TOTP_Error, OnClickShowDatabase)
                {
                    Tag = pwDocument
                }
            };
        }

        protected ToolStripMenuItem CreateMenuItemFromPwEntry(PwEntry entry, PwDatabase pwDatabase)
        {
            var context = new SprContext(entry, pwDatabase, SprCompileFlags.All, false, false);
            var entryTitle = entry.Strings.ReadSafe(PwDefs.TitleField);
            var entryUsername = SprEngine.Compile(entry.Strings.ReadSafe(PwDefs.UserNameField), context);

            string trayTitle = TrimMenuItemTitleIfNecessary(entryTitle, entryUsername);

            var validEntry = Plugin.TOTPEntryValidator.SettingsValidate(entry);
            var menuItem = new ToolStripMenuItem(trayTitle)
            {
                Tag = entry,
                Image = validEntry ? GetEntryImage(entry, pwDatabase) : Resources.TOTP_Error,
                BackColor = entry.BackgroundColor,
                ForeColor = entry.ForegroundColor,
                Enabled = validEntry,
            };
            menuItem.Click += OnNotifyMenuTOTPClick;

            return menuItem;
        }

        private Image GetEntryImage(PwEntry pe, PwDatabase pwDatabase)
        {
            if (pwDatabase != null)
            {
                if (!pe.CustomIconUuid.Equals(PwUuid.Zero))
                {
                    int w = DpiUtil.ScaleIntX(16);
                    int h = DpiUtil.ScaleIntY(16);

                    return pwDatabase.GetCustomIcon(pe.CustomIconUuid, w, h);
                }
                var iconId = (int)pe.IconId;
                if (PluginHost.MainWindow.ClientIcons != null &&
                    PluginHost.MainWindow.ClientIcons.Images != null &&
                    PluginHost.MainWindow.ClientIcons.Images.Count > iconId)
                {
                    return PluginHost.MainWindow.ClientIcons.Images[iconId];
                }
            }

            return Resources.TOTP_Key;
        }

        private string TrimMenuItemTitleIfNecessary(string entryTitle, string entryUsername)
        {
            if (string.IsNullOrEmpty(entryUsername) ||
                (Plugin.Settings.TrimTrayText &&
                 entryTitle.Length + entryUsername.Length > Plugin.Settings.TrimTextLength))
            {
                return entryTitle.ExtWithSpaceAfter();
            }

            return entryTitle.ExtWithSpaceAfter() + entryUsername.ExtWithParenthesis();
        }

        protected void OnNotifyMenuTOTPClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
            {
                return;
            }

            var pe = menuItem.Tag as PwEntry;
            if (pe != null)
            {
                Plugin.TOTPCopyToClipboard(pe);
            }
        }
    }
}