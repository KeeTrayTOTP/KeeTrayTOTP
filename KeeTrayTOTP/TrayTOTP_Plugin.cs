using KeePass.App.Configuration;
using KeePass.Plugins;
using KeePass.UI;
using KeePass.Util;
using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Utility;
using KeeTrayTOTP.Helpers;
using KeeTrayTOTP.Libraries;
using KeeTrayTOTP.Menu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
        /// Previous value of KeePass.Program.Config.MainWindow.ShowEntriesOfSubGroups
        /// </summary>
        private bool _bPreviousShowEntriesOfSubGroups;

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

            PluginHost.MainWindow.UIStateUpdated += MainWindow_UIStateUpdated;

            return true;
        }

        /// <summary>
        /// React on the user toggling display of entries in subgroups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_UIStateUpdated(object sender, EventArgs e)
        {
            if (_bPreviousShowEntriesOfSubGroups != KeePass.Program.Config.MainWindow.ShowEntriesOfSubGroups)
            {
                //User toggled display of entries in subgroups
                //Enforce rechecking for required updates in OnTimerTick
                _bPreviousShowEntriesOfSubGroups = KeePass.Program.Config.MainWindow.ShowEntriesOfSubGroups;
                ResetLastSelectedGroup();
            }
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
            return GetVisibleAndValidPasswordEntries(PluginHost.MainWindow.ActiveDatabase);
        }

        /// <summary>
        /// Get all the password entries in all groups and filter entries that are expired or have invalid TOTP settings.
        /// </summary>
        internal IEnumerable<PwEntry> GetVisibleAndValidPasswordEntries(PwDatabase pwDatabase)
        {
            var entries = pwDatabase.RootGroup.GetEntries(true);
            var inRecycleBinFunc = CreateInRecycleBinFunc(pwDatabase);

            return entries.Where(entry => !entry.IsExpired() && TOTPEntryValidator.HasSeed(entry) && !inRecycleBinFunc(entry));
        }

        /// <summary>
        /// Create an optimal function for checking whether an entry is in the recycle bin
        /// </summary>
        /// <remarks>
        /// Returns a func to prevent looking up the recycle bin for every entry.
        /// </remarks>
        /// <param name="pwDatabase"></param>
        private static Func<PwEntry, bool> CreateInRecycleBinFunc(PwDatabase pwDatabase)
        {
            if (pwDatabase.RecycleBinEnabled)
            {
                var pgRecycleBin = pwDatabase.RootGroup.FindGroup(pwDatabase.RecycleBinUuid, true);
                if (pgRecycleBin != null)
                {
                    return (PwEntry entry) => entry.IsContainedIn(pgRecycleBin);
                }
            }

            return (PwEntry entry) => false;
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
                        foreach (var entry in selectedGroup.GetEntries(KeePass.Program.Config.MainWindow.ShowEntriesOfSubGroups))
                        {
                            if (TOTPEntryValidator.HasSeed(entry))
                            {
                                _liColumnTotpContains = true;
                                break; //No need to check remaining entries
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

                        TOTPProvider totpGenerator = new TOTPProvider(settings, this.TimeCorrections);

                        string invalidCharacters;

                        if (TOTPEntryValidator.SeedValidate(e.Context.Entry, out invalidCharacters))
                        {
                            e.Context.Entry.Touch(false);
                            string totp = totpGenerator.GenerateByByte(Base32.Decode(TOTPEntryValidator.SeedGet(e.Context.Entry).ReadString().ExtWithoutSpaces()));
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
                    string[] settings = TOTPEntryValidator.SettingsGet(pe);

                    TOTPProvider totpGenerator = new TOTPProvider(settings, this.TimeCorrections);

                    string invalidCharacters;
                    if (TOTPEntryValidator.SeedValidate(pe, out invalidCharacters))
                    {
                        pe.Touch(false);

                        string totp = totpGenerator.Generate(TOTPEntryValidator.SeedGet(pe).ReadString().ExtWithoutSpaces());

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
            PluginHost.MainWindow.UIStateUpdated -= MainWindow_UIStateUpdated;

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

        /// <summary>
        /// Resets last selected group to ensure TOTP is shown and TOTP value counter is active
        /// </summary>
        internal void ResetLastSelectedGroup()
        {
            _liGroupsPreviousSelected = null;
        }
    }
}
