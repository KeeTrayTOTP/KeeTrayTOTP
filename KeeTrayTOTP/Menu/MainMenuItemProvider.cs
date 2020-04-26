using System;
using System.Windows.Forms;
using KeePass.UI;

namespace KeeTrayTOTP.Menu
{
    public class MainMenuItemProvider : IMenuItemProvider
    {
        private readonly KeeTrayTOTPExt _plugin;

        public MainMenuItemProvider(KeeTrayTOTPExt plugin)
        {
            _plugin = plugin;
        }

        private void OnMenuSettingsClick(object sender, EventArgs e)
        {
            UIUtil.ShowDialogAndDestroy(new FormSettings(_plugin));
        }

        private void OnMenuAboutClick(object sender, EventArgs e)
        {
            UIUtil.ShowDialogAndDestroy(new FormAbout());
        }

        public ToolStripMenuItem ProvideMenuItem()
        {
            var rootMainMenuItem =
                new ToolStripMenuItem(Localization.Strings.TrayTOTPPlugin, Properties.Resources.TOTP);

            var mainMenuSettings = new ToolStripMenuItem(Localization.Strings.Settings,
                Properties.Resources.TOTP_Settings, OnMenuSettingsClick);
            var mainMenuSeparator = new ToolStripSeparator();
            var mainMenuHelp = new ToolStripMenuItem(Localization.Strings.Help, Properties.Resources.TOTP_Help,
                _plugin.OnMenuHelpClick);
            var mainMenuAbout = new ToolStripMenuItem(Localization.Strings.About + "...",
                Properties.Resources.TOTP_Info, OnMenuAboutClick);

            rootMainMenuItem.DropDownItems.Add(mainMenuSettings);
            rootMainMenuItem.DropDownItems.Add(mainMenuSeparator);
            rootMainMenuItem.DropDownItems.Add(mainMenuHelp);
            rootMainMenuItem.DropDownItems.Add(mainMenuAbout);

            return rootMainMenuItem;
        }
    }
}