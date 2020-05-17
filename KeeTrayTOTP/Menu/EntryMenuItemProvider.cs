using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeeTrayTOTP.Helpers;

namespace KeeTrayTOTP.Menu
{
    public class EntryMenuItemProvider : IMenuItemProvider
    {
        private readonly KeeTrayTOTPExt _plugin;
        private readonly IPluginHost _pluginHost;

        public EntryMenuItemProvider(KeeTrayTOTPExt plugin, IPluginHost pluginHost)
        {
            _plugin = plugin;
            _pluginHost = pluginHost;
        }

        public ToolStripMenuItem ProvideMenuItem()
        {
            var rootEntryMenuItem = new ToolStripMenuItem(Localization.Strings.TrayTOTPPlugin, Properties.Resources.TOTP);

            // The ShortCutKeys assignments are required in this form and
            // can not be setup as constructor parameters, because an implementation
            // in the mono framework is missing.
            // see https://github.com/mono/mono/issues/19755
            var entryMenuCopyTotp = new ToolStripMenuItem(Localization.Strings.CopyTOTP, Properties.Resources.TOTP, OnEntryMenuTOTPClick)
            {
                ShortcutKeys = (Keys)Shortcut.CtrlT
            };
            var entryMenuSetupTotp = new ToolStripMenuItem(Localization.Strings.SetupTOTP, Properties.Resources.TOTP_Setup, OnEntryMenuSetupClick)
            {
                ShortcutKeys = (Keys)Shortcut.CtrlShiftI
            };
            var entryMenuShowQrCode = new ToolStripMenuItem(Localization.Strings.ShowQR, Properties.Resources.TOTP_Setup, OnEntryMenuShowQRClick)
            {
                ShortcutKeys = (Keys)Shortcut.CtrlShiftJ
            };
            
            rootEntryMenuItem.DropDownItems.Add(entryMenuCopyTotp);
            rootEntryMenuItem.DropDownItems.Add(entryMenuSetupTotp);
            rootEntryMenuItem.DropDownItems.Add(entryMenuShowQrCode);
            rootEntryMenuItem.DropDownOpening += (sender, _) =>
            {
                entryMenuCopyTotp.Enabled = false;
                entryMenuSetupTotp.Enabled = false;
                entryMenuShowQrCode.Enabled = false;

                entryMenuCopyTotp.Visible = _plugin.Settings.EntryContextCopyVisible;
                entryMenuSetupTotp.Visible = _plugin.Settings.EntryContextSetupVisible;

                if (_pluginHost.MainWindow.GetSelectedEntriesCount() == 1)
                {
                    var currentEntry = _pluginHost.MainWindow.GetSelectedEntry(true);
                    if (_plugin.TOTPEntryValidator.HasSeed(currentEntry) &&
                        _plugin.TOTPEntryValidator.SettingsValidate(currentEntry))
                    {
                        entryMenuCopyTotp.Enabled = true;
                        entryMenuShowQrCode.Enabled = true;
                    }

                    entryMenuSetupTotp.Enabled = true;
                }
            };

            rootEntryMenuItem.DropDownClosed += (sender, args) => entryMenuCopyTotp.Enabled = true;

            return rootEntryMenuItem;
        }

        private void OnEntryMenuSetupClick(object sender, EventArgs e)
        {
            if (_pluginHost.MainWindow.GetSelectedEntriesCount() == 1)
            {
                UIUtil.ShowDialogAndDestroy(new SetupTOTP(_plugin, _pluginHost.MainWindow.GetSelectedEntry(true)));
                _pluginHost.MainWindow.RefreshEntriesList();
            }
        }

        private void OnEntryMenuShowQRClick(object sender, EventArgs e)
        {
            if (_pluginHost.MainWindow.GetSelectedEntriesCount() != 1)
            {
                return;
            }

            var entry = _pluginHost.MainWindow.GetSelectedEntry(true);

            if (!_plugin.TOTPEntryValidator.HasSeed(entry))
            {
                return;
            }

            var rawSeed = _plugin.TOTPEntryValidator.SeedGet(entry).ReadString();
            var cleanSeed = Regex.Replace(rawSeed, @"\s+", "").TrimEnd('=');
            var issuer = entry.Strings.Get("Title").ReadString();
            var username = entry.Strings.Get("UserName").ReadString();
            UIUtil.ShowDialogAndDestroy(new ShowQR(cleanSeed, issuer, username));

            _pluginHost.MainWindow.RefreshEntriesList();
        }

        private void OnEntryMenuTOTPClick(object sender, EventArgs e)
        {
            PwEntry pe = _pluginHost.MainWindow.GetSelectedEntry(false);

            if (pe != null)
            {
                _plugin.TOTPCopyToClipboard(pe);
            }
        }
    }
}