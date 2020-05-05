using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using KeePass.App.Configuration;
using KeePass.Plugins;
using KeePass.UI;
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Utility;
using KeeTrayTOTP.Libraries;
using KeeTrayTOTP.Menu;
using KeeTrayTOTP.Helpers;
using KeePass.Util;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Main Plugin Class
    /// </summary>
    public sealed class KeeTrayTOTPExt : Plugin
    {
        /// <summary>
        /// Plugin host Global Reference for access to KeePass functions.
        /// </summary>
        internal IPluginHost PluginHost;

        /// <summary>
        /// Constants (keepass form object names).
        /// </summary>
        internal const string keeobj_string_EntryContextMenuCopyPassword_Name = "m_ctxEntryCopyPassword";
        internal const string keeobj_string_EntryContextMenuEntriesSubMenu_Name = "m_ctxEntryMassModify";
        internal const string keeobj_string_EntryContextMenuEntriesSubMenuSeperator1_Name = "m_ctxEntrySelectedSep1";

        /// <summary>
        /// Form Help Global Reference.
        /// </summary>
        private FormHelp _helpForm;

        /// <summary>
        /// Provides columns to KeePass
        /// </summary>
        private ColumnProvider columnProvider;

        /// <summary>
        /// Entry List Column Count.
        /// </summary>
        private int _liColumnsCount;
        /// <summary>
        /// Entry List Column TOTP visibility.
        /// </summary>
        private bool _liColumnTotpVisible;
        /// <summary>
        /// Entry Groups last selected group.
        /// </summary>
        private PwGroup _liGroupsPreviousSelected;
        /// <summary>
        /// Entry Column TOTP has TOTPs.
        /// </summary>
        private bool _liColumnTotpContains;

        /// <summary>
        /// Entries Refresh Timer.
        /// </summary>
        private readonly Timer _liRefreshTimer = new Timer();

        /// <summary>
        /// Entries Refresh Timer Previous Counter to Prevent Useless Refresh.
        /// </summary>
        private int _liRefreshTimerPreviousCounter;

        /// <summary>
        /// This provider is used for providing menu items to the keepass host.
        /// </summary>
        private MenuItemProvider _menuItemProvider;

        /// <summary>
        /// Time Correction Collection.
        /// </summary>
        internal TimeCorrectionCollection TimeCorrections;

        /// <summary>
        /// Network status of the computer.
        /// </summary>
        internal bool NetworkIsConnected
        {
            get
            {
                return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            }
        }

        public Settings Settings { get; private set; }

        public TOTPEntryValidator TOTPEntryValidator { get; private set; }

        public override ToolStripMenuItem GetMenuItem(PluginMenuType type)
        {
            if (_menuItemProvider != null)
            {
                return _menuItemProvider.GetMenuItem(type);
            }

            return null;
        }

        /// <summary>
        /// Initialization of the plugin, adding menus, handlers and forms.
        /// </summary>
        /// <param name="host">Plugin host for access to KeePass functions.</param>
        /// <returns>Successful loading of the plugin, if not the plugin is removed.</returns>
        public override bool Initialize(IPluginHost host)
        {
            // Internalize Host Handle.
            if (host == null)
            {
                return false;
            }

            PluginHost = host;

            Settings = new Settings(host.CustomConfig);
            TOTPEntryValidator = new TOTPEntryValidator(Settings);

            _menuItemProvider = new MenuItemProvider(this, PluginHost);

            // Instantiate Help Form.
            _helpForm = new FormHelp();

            // Register events.
            PluginHost.MainWindow.Shown += MainWindow_Shown;

            // Register auto-type function.
            if (Settings.AutoTypeEnable)
            {
                SprEngine.FilterCompile += SprEngine_FilterCompile;
                SprEngine.FilterPlaceholderHints.Add(Settings.AutoTypeFieldName.ExtWithBrackets());
            }

            // List Column TOTP.
            columnProvider = new TrayTOTP_ColumnProvider(this);
            PluginHost.ColumnProviderPool.Add(columnProvider);

            // Refresh Timer.
            _liRefreshTimer.Interval = Settings.EntryListRefreshRate;
            _liRefreshTimer.Enabled = true;
            _liRefreshTimer.Tick += OnTimerTick;

            //Time Correction.
            TimeCorrectionProvider.Interval = Convert.ToInt16(Settings.TimeCorrectionRefreshTime);
            TimeCorrections = new TimeCorrectionCollection(Settings.TimeCorrectionEnable);
            TimeCorrections.AddRangeFromList(Settings.TimeCorrectionList);

            return true;
        }


        /// <summary>
        /// Occurs when the main window is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (!Settings.FirstInstallShown)
            {
                Settings.FirstInstallShown = true;
                if (!_helpForm.Visible)
                {
                    _helpForm = new FormHelp(true);
                    _helpForm.Show();
                }
                else
                {
                    _helpForm.Focus();
                }
            }
        }

        /// <summary>
        /// Tools Menu Tray TOTP Help Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnMenuHelpClick(object sender, EventArgs e)
        {
            if (!_helpForm.Visible)
            {
                _helpForm.Show();
            }
            else
            {
                _helpForm.Focus();
            }
        }

        /// <summary>
        /// Get all the password entries in all groups and filter entries that are expired or have invalid TOTP settings.
        /// </summary>
        internal IEnumerable<PwEntry> GetVisibleAndValidPasswordEntries()
        {
            return GetVisibleAndValidPasswordEntries(PluginHost.MainWindow.ActiveDatabase.RootGroup);
        }

        /// <summary>
        /// Get all the password entries in all groups and filter entries that are expired or have invalid TOTP settings.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<PwEntry> GetVisibleAndValidPasswordEntries(PwGroup pwGroup)
        {
            var entries = pwGroup.GetEntries(true);

            return entries.Where(entry => !entry.IsExpired() && TOTPEntryValidator.HasSeed(entry));
        }

        /// <summary>
        /// Timer Event that occurs to refresh the entry list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (PluginHost.MainWindow.ActiveDatabase.IsOpen && PluginHost.MainWindow.Visible)
            {
                if (KeePass.Program.Config.MainWindow.EntryListColumns.Count != _liColumnsCount)
                {
                    _liColumnTotpVisible = false;
                    _liColumnsCount = KeePass.Program.Config.MainWindow.EntryListColumns.Count;
                    foreach (var column in KeePass.Program.Config.MainWindow.EntryListColumns)
                    {
                        if (column.Type == AceColumnType.PluginExt && column.CustomName == Localization.Strings.TOTP)
                        {
                            _liColumnTotpVisible = true;
                        }
                    }
                }

                if (_liColumnTotpVisible)
                {
                    PwGroup selectedGroup = PluginHost.MainWindow.GetSelectedGroup();
                    if (selectedGroup != _liGroupsPreviousSelected)
                    {
                        _liColumnTotpContains = false;
                        _liGroupsPreviousSelected = selectedGroup;
                        foreach (var entry in selectedGroup.GetEntries(true))
                        {
                            if (TOTPEntryValidator.HasSeed(entry))
                            {
                                _liColumnTotpContains = true;
                            }
                        }
                    }
                }

                if (_liColumnTotpVisible && _liColumnTotpContains) //Tests if displayed entries have totps that require refreshing.
                {
                    var currentSeconds = DateTime.Now.Second;
                    if (_liRefreshTimerPreviousCounter != currentSeconds)
                    {
                        PluginHost.MainWindow.RefreshEntriesList();
                        _liRefreshTimerPreviousCounter = currentSeconds;
                    }
                }
            }
        }

        /// <summary>
        /// Auto-Type Function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SprEngine_FilterCompile(object sender, SprEventArgs e)
        {
            if ((e.Context.Flags & SprCompileFlags.ExtActive) == SprCompileFlags.ExtActive && e.Text.IndexOf(Settings.AutoTypeFieldName.ExtWithBrackets(), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                if (TOTPEntryValidator.HasSeed(e.Context.Entry))
                {
                    if (TOTPEntryValidator.SettingsValidate(e.Context.Entry))
                    {
                        string[] settings = TOTPEntryValidator.SettingsGet(e.Context.Entry);
                        var key = Base32.Decode(TOTPEntryValidator.SeedGet(e.Context.Entry).ReadString().ExtWithoutSpaces());
                        TOTPProvider totpGenerator = new TOTPProvider(settings, key, this.TimeCorrections);

                        string invalidCharacters;

                        if (TOTPEntryValidator.SeedValidate(e.Context.Entry, out invalidCharacters))
                        {
                            e.Context.Entry.Touch(false);
                            var totp = totpGenerator.Generate();
                            e.Text = StrUtil.ReplaceCaseInsensitive(e.Text, Settings.AutoTypeFieldName.ExtWithBrackets(), totp);
                        }
                        else
                        {
                            e.Text = string.Empty;
                            MessageService.ShowWarning(Localization.Strings.ErrorBadSeed + invalidCharacters.ExtWithParenthesis().ExtWithSpaceBefore());
                        }
                        if (totpGenerator.TimeCorrectionError)
                        {
                            MessageService.ShowWarning(Localization.Strings.WarningBadURL);
                        }
                    }
                    else
                    {
                        e.Text = string.Empty;
                        MessageService.ShowWarning(Localization.Strings.ErrorBadSettings);
                    }
                }
                else
                {
                    e.Text = string.Empty;
                    MessageService.ShowWarning(Localization.Strings.ErrorNoSeed);
                }
            }
        }

        /// <summary>
        /// Copies the specified entry's generated TOTP to the clipboard using the KeePass's clipboard function.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        internal void TOTPCopyToClipboard(PwEntry pe)
        {
            if (TOTPEntryValidator.HasSeed(pe))
            {
                if (TOTPEntryValidator.SettingsValidate(pe))
                {
                    var settings = TOTPEntryValidator.SettingsGet(pe);
                    var key = Base32.Decode(TOTPEntryValidator.SeedGet(pe).ReadString().ExtWithoutSpaces());
                    TOTPProvider totpGenerator = new TOTPProvider(settings, key, this.TimeCorrections);

                    string invalidCharacters;
                    if (TOTPEntryValidator.SeedValidate(pe, out invalidCharacters))
                    {
                        pe.Touch(false);

                        var totp = totpGenerator.Generate();

                        ClipboardUtil.CopyAndMinimize(totp, true, PluginHost.MainWindow, pe, PluginHost.MainWindow.ActiveDatabase);
                        PluginHost.MainWindow.StartClipboardCountdown();
                    }
                    else
                    {
                        MessageService.ShowWarning(Localization.Strings.ErrorBadSeed + invalidCharacters.ExtWithParenthesis().ExtWithSpaceBefore());
                    }
                    if (totpGenerator.TimeCorrectionError)
                    {
                        MessageService.ShowWarning(Localization.Strings.WarningBadURL);
                    }
                }
                else
                {
                    MessageService.ShowWarning(Localization.Strings.ErrorBadSettings);
                }
            }
            else
            {
                MessageService.ShowWarning(Localization.Strings.ErrorNoSeed);
            }
        }

        /// <summary>
        /// Removes all resources such as menus, handles and forms from KeePass.
        /// </summary>
        public override void Terminate()
        {
            // Destroy Help Form.
            if (_helpForm.Visible)
            {
                _helpForm.Close();
            }
            else
            {
                _helpForm.Dispose();
            }

            // Unregister internal events.
            PluginHost.MainWindow.Shown -= MainWindow_Shown;

            // Dispose menu items
            if (_menuItemProvider != null)
            {
                _menuItemProvider.Dispose();
            }

            // Unregister auto-type function.
            if (SprEngine.FilterPlaceholderHints.Contains(Settings.AutoTypeFieldName.ExtWithBrackets()))
            {
                SprEngine.FilterCompile -= SprEngine_FilterCompile;
                SprEngine.FilterPlaceholderHints.Remove(Settings.AutoTypeFieldName.ExtWithBrackets());
            }

            // Remove Column provider.
            PluginHost.ColumnProviderPool.Remove(columnProvider);
            columnProvider = null;

            // Remove Timer.
            _liRefreshTimer.Tick -= OnTimerTick;
            _liRefreshTimer.Dispose();
        }

        /// <summary>
        /// Returns the image of the plugin to display in the KeePass plugin list.
        /// </summary>
        public override Image SmallIcon
        {
            get { return Properties.Resources.TOTP; }
        }

        /// <summary>
        /// Returns update URL for KeepAss automatic update check. (file must be UTF-8 without BOM (support for BOM from KP 2.21))
        /// </summary>
        public override string UpdateUrl
        {
            get { return "https://raw.githubusercontent.com/KeeTrayTOTP/KeeTrayTOTP/master/version_manifest.txt"; }
        }
    }
}
