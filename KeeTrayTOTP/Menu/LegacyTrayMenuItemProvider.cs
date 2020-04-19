using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.Util.Spr;
using KeePassLib;
using KeeTrayTOTP.Libraries;
using KeeTrayTOTP.Localization;
using KeeTrayTOTP.Properties;

namespace KeeTrayTOTP.Menu
{
    /// <summary>
    ///     This MenuItemProvider provides the legacy implementation of the KeeTrayTOTP plugin.
    ///     It will place each totp enabled entry in the root dropdown menu.
    ///     This is not recommended as it is using an approach that is not recommended from the keepass author.
    ///     see https://keepass.info/help/v2_dev/plg_index.html#co_menuitem
    /// </summary>
    /// <remarks>
    ///     The provider has to keep track of the items itself, as they are not handled by keepass,
    ///     cause we "inject" them and bypass the intended way.
    /// </remarks>
    public class LegacyTrayMenuItemProvider : MenuItemProviderBase
    {
        private ToolStripMenuItem _niMenuTitle;
        private ToolStripSeparator _niMenuSeparator;

        private readonly List<ToolStripMenuItem> _niMenuList = new List<ToolStripMenuItem>();
        protected KeeTrayTOTPExt Plugin;
        protected IPluginHost PluginHost;

        public LegacyTrayMenuItemProvider(KeeTrayTOTPExt plugin, IPluginHost pluginHost)
        {
            PluginHost = pluginHost;
            Plugin = plugin;
            SetUpLegacyContextMenuEntries();
        }

        private void SetUpLegacyContextMenuEntries()
        {
            _niMenuTitle = new ToolStripMenuItem(Strings.TrayTOTPPlugin, Resources.TOTP);
            _niMenuSeparator = new ToolStripSeparator();

            PluginHost.MainWindow.TrayContextMenu.Items.Insert(0, _niMenuTitle);
            PluginHost.MainWindow.TrayContextMenu.Items.Insert(1, _niMenuSeparator);

            PluginHost.MainWindow.TrayContextMenu.Opening += OnNotifyMenuOpening;
            PluginHost.MainWindow.TrayContextMenu.Opened += OnTrayContextMenuOpened;
        }

        public override ToolStripMenuItem ProvideMenuItem()
        {
            return null;
        }

        private void OnTrayContextMenuOpened(object sender, EventArgs e)
        {
            var contextMenuStrip = (ContextMenuStrip)sender;
            var dropDownLocationCalculator = new DropDownLocationCalculator(contextMenuStrip.Size);
            contextMenuStrip.Location = dropDownLocationCalculator.CalculateLocationForDropDown(Cursor.Position);
        }

        private void OnNotifyMenuOpening(object sender, CancelEventArgs e)
        {
            foreach (var menu in _niMenuList)
            {
                PluginHost.MainWindow.TrayContextMenu.Items.Remove(menu);
            }

            _niMenuList.Clear();
            if (PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_NotifyContext_Visible, true))
            {
                _niMenuTitle.Visible = true;
                _niMenuSeparator.Visible = true;
                if (PluginHost.MainWindow.ActiveDatabase.IsOpen)
                {
                    var trimTrayText = PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);
                    foreach (PwEntry entry in Plugin.GetVisibleAndValidPasswordEntries())
                    {
                        var entryTitle = entry.Strings.ReadSafe(PwDefs.TitleField);

                        var context = new SprContext(entry, PluginHost.MainWindow.ActiveDatabase, SprCompileFlags.All, false, false);
                        var entryUsername = SprEngine.Compile(entry.Strings.ReadSafe(PwDefs.UserNameField), context);
                        string trayTitle;
                        if ((trimTrayText && entryTitle.Length + entryUsername.Length > KeeTrayTOTPExt.setstat_trim_text_length) || string.IsNullOrEmpty(entryUsername))
                        {
                            trayTitle = entryTitle.ExtWithSpaceAfter();
                        }
                        else
                        {
                            trayTitle = entryTitle.ExtWithSpaceAfter() + entryUsername.ExtWithParenthesis();
                        }
                        PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);

                        var newMenu = new ToolStripMenuItem(trayTitle, Properties.Resources.TOTP_Key, OnNotifyMenuTOTPClick);
                        newMenu.Tag = entry;
                        if (!Plugin.SettingsValidate(entry))
                        {
                            newMenu.Enabled = false;
                            newMenu.Image = Properties.Resources.TOTP_Error;
                        }
                        _niMenuList.Add(newMenu);
                    }

                    if (_niMenuList.Count > 0)
                    {
                        _niMenuList.Sort((p1, p2) => string.Compare(p1.Text, p2.Text, true));
                        for (int i = 0; i <= _niMenuList.Count - 1; i++)
                        {
                            PluginHost.MainWindow.TrayContextMenu.Items.Insert(i + 1, _niMenuList[i]);
                        }
                    }
                    else
                    {
                        var newMenu = new ToolStripMenuItem(Strings.NoTOTPSeedFound);
                        newMenu.Image = Resources.TOTP_None;
                        _niMenuList.Add(newMenu);
                        PluginHost.MainWindow.TrayContextMenu.Items.Insert(1, _niMenuList[0]);
                    }

                    CreateMenuItemForOtherDatabases(_niMenuList);
                }
                else
                {
                    if (PluginHost.MainWindow.IsFileLocked(null))
                    {
                        var newMenu = new ToolStripMenuItem(Strings.DatabaseIsLocked);
                        newMenu.Image = Resources.TOTP_Lock;
                        _niMenuList.Add(newMenu);
                        PluginHost.MainWindow.TrayContextMenu.Items.Insert(1, _niMenuList[0]);
                    }
                    else
                    {
                        var newMenu = new ToolStripMenuItem(Strings.DatabaseIsNotOpen);
                        newMenu.Image = Resources.TOTP_Error;
                        _niMenuList.Add(newMenu);
                        PluginHost.MainWindow.TrayContextMenu.Items.Insert(1, _niMenuList[0]);
                    }
                }
            }
            else
            {
                _niMenuTitle.Visible = false;
                _niMenuSeparator.Visible = false;
            }
        }

        private void CreateMenuItemForOtherDatabases(IList<ToolStripMenuItem> items)
        {
            var tabcontrol = PluginHost.MainWindow.Controls.OfType<TabControl>().FirstOrDefault();
            var nonSelectedTabs =
                tabcontrol.TabPages.OfType<TabPage>().Where(c => c != tabcontrol.SelectedTab).ToList();

            int i = 1;
            foreach (var tab in nonSelectedTabs)
            {
                var item = new ToolStripMenuItem(string.Format(Strings.SwitchTo, tab.Text)) {Tag = tab};
                item.Click += SwitchToOtherDatabase;
                items.Add(item);

                PluginHost.MainWindow.TrayContextMenu.Items.Insert(i++, item);
            }
        }

        private void SwitchToOtherDatabase(object sender, EventArgs e)
        {
            var tabControl = PluginHost.MainWindow.Controls.OfType<TabControl>().FirstOrDefault();
            var changeDbToolStripItem = sender as ToolStripMenuItem;
            if (changeDbToolStripItem != null)
            {
                var databaseTab = changeDbToolStripItem.Tag as TabPage;
                if (databaseTab != null)
                {
                    tabControl.SelectedTab = databaseTab;
                }
            }
        }

        public override void Dispose()
        {
            // Remove Notify Icon menus.
            PluginHost.MainWindow.TrayContextMenu.Opening -= OnNotifyMenuOpening;
            PluginHost.MainWindow.TrayContextMenu.Opened -= OnTrayContextMenuOpened;

            PluginHost.MainWindow.TrayContextMenu.Items.Remove(_niMenuTitle);
            _niMenuTitle.Dispose();
            foreach (var menu in _niMenuList)
            {
                PluginHost.MainWindow.TrayContextMenu.Items.Remove(menu);
                menu.Dispose();
            }

            PluginHost.MainWindow.TrayContextMenu.Items.Remove(_niMenuSeparator);
            _niMenuSeparator.Dispose();
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
    }
}