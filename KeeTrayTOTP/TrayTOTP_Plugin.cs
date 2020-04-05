using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using KeePass.App.Configuration;
using KeePass.Plugins;
using KeePass.UI;
using KeePass.Util;
using KeePass.Util.Spr;

using KeePassLib;
using KeePassLib.Utility;
using KeePassLib.Security;
using KeeTrayTOTP.Libraries;
using KeeTrayTOTP.Localization;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Main Plugin Class
    /// </summary>
    public sealed partial class KeeTrayTOTPExt : Plugin
    {
        /// <summary>
        /// Plugin host Global Reference for access to KeePass functions.
        /// </summary>
        internal IPluginHost PluginHost = null;

        /// <summary>
        /// Tray TOTP Support Email
        /// </summary>
        internal const string Email = "victor.rds@protonmail.ch";

        /// <summary>
        /// Constants (keepass form object names).
        /// </summary>
        internal const string keeobj_string_EntryContextMenuCopyPassword_Name = "m_ctxEntryCopyPassword";
        internal const string keeobj_string_EntryContextMenuEntriesSubMenu_Name = "m_ctxEntryMassModify";
        internal const string keeobj_string_EntryContextMenuEntriesSubMenuSeperator1_Name = "m_ctxEntrySelectedSep1";

        /// <summary>
        /// Constants (custom string key).
        /// </summary>
        internal const string setname_string_TimeCorrection_List = "traytotp_timecorrection_list";

        /// <summary>
        /// Constants (setting names).
        /// </summary>
        internal const string setname_bool_FirstInstall_Shown = "firstinstall_shown";
        internal const string setname_bool_EntryContextCopy_Visible = "entrycontextcopy_visible";
        internal const string setname_bool_EntryContextSetup_Visible = "entrycontextsetup_visible";
        internal const string setname_bool_NotifyContext_Visible = "notifycontext_visible";
        internal const string setname_bool_TOTPColumnCopy_Enable = "totpcolumncopy_enable";
        internal const string setname_bool_TOTPColumnTimer_Visible = "totpcolumntimer_visible";
        internal const string setname_bool_AutoType_Enable = "autotype_enable";
        internal const string setname_string_AutoType_FieldName = "autotype_fieldname";
        internal const string setname_bool_TimeCorrection_Enable = "timecorrection_enable";
        internal const string setname_ulong_TimeCorrection_RefreshTime = "timecorrection_refreshtime";
        internal const string setname_string_TOTPSeed_StringName = "totpseed_stringname";
        internal const string setname_string_TOTPSettings_StringName = "totpsettings_stringname";
        internal const string setname_bool_TrimTrayText = "traytotp_trim_tray_text";

        /// <summary>
        /// Constants (default settings values).
        /// </summary>
        internal const string setdef_string_AutoType_FieldName = "TOTP";
        internal const long setdef_ulong_TimeCorrection_RefreshTime = 60;

        /// <summary>
        /// Constants (static settings value).
        /// </summary>
        internal const int setstat_int_EntryList_RefreshRate = 300;
        internal const int setstat_trim_text_length = 25;
        internal IReadOnlyList<string> setstat_allowed_lengths = new ReadOnlyCollection<string>(new[] { "6", "7", "8", "S" });

        /// <summary>
        /// Form Help Global Reference.
        /// </summary>
        private FormHelp _helpForm;

        /// <summary>
        /// Notify Icon Context Menu Title.
        /// </summary>
        private ToolStripMenuItem _niMenuTitle;

        /// <summary>
        /// Notify Icon Context Menu List.
        /// </summary>
        private List<ToolStripMenuItem> _niMenuList = new List<ToolStripMenuItem>();
        /// <summary>
        /// Notify Icon Context Menu Separator.
        /// </summary>
        private ToolStripSeparator _niMenuSeperator = null;

        /// <summary>
        /// Entries Column TOTP.
        /// </summary>
        private TrayTOTP_CustomColumn _liColumnTotp = null;

        /// <summary>
        /// Entry List Column Count.
        /// </summary>
        private int _liColumnsCount = 0;
        /// <summary>
        /// Entry List Column TOTP visibility.
        /// </summary>
        private bool _liColumnTotpVisible = false;
        /// <summary>
        /// Entry Groups last selected group.
        /// </summary>
        private PwGroup _liGroupsPreviousSelected = null;
        /// <summary>
        /// Entry Column TOTP has TOTPs.
        /// </summary>
        private bool _liColumnTotpContains = false;

        /// <summary>
        /// Entries Refresh Timer.
        /// </summary>
        private Timer _liRefreshTimer = new Timer();

        /// <summary>
        /// Entries Refresh Timer.
        /// </summary>
        internal int LiRefreshTimerInterval
        {
            set
            {
                _liRefreshTimer.Interval = value;
                _liRefreshTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Entries Refresh Timer Previous Counter to Prevent Useless Refresh.
        /// </summary>
        private int _liRefreshTimerPreviousCounter;

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

        public override ToolStripMenuItem GetMenuItem(PluginMenuType type)
        {
            // Provide a menu item for the main location(s)
            if (type == PluginMenuType.Main)
            {
                var toMenuTrayTOTP = new ToolStripMenuItem(Localization.Strings.TrayTOTPPlugin);
                toMenuTrayTOTP.Image = Properties.Resources.TOTP;

                var toSubMenuSettings = new ToolStripMenuItem(Localization.Strings.Settings);
                toSubMenuSettings.Image = Properties.Resources.TOTP_Settings;
                toSubMenuSettings.Click += OnMenuSettingsClick;

                toMenuTrayTOTP.DropDownItems.Add(toSubMenuSettings);
                var toSubMenuSeparator1 = new ToolStripSeparator();
                toMenuTrayTOTP.DropDownItems.Add(toSubMenuSeparator1);
                var toSubMenuHelp = new ToolStripMenuItem(Localization.Strings.Help);
                toSubMenuHelp.Image = Properties.Resources.TOTP_Help;
                toSubMenuHelp.Click += OnMenuHelpClick;
                toMenuTrayTOTP.DropDownItems.Add(toSubMenuHelp);
                var toSubMenuAbout = new ToolStripMenuItem(Localization.Strings.About + "...");
                toSubMenuAbout.Image = Properties.Resources.TOTP_Info;
                toSubMenuAbout.Click += OnMenuAboutClick;
                toMenuTrayTOTP.DropDownItems.Add(toSubMenuAbout);

                return toMenuTrayTOTP;
            }
            else if (type == PluginMenuType.Entry)
            {
                var enMenuTrayTotp = new ToolStripMenuItem(Localization.Strings.TrayTOTPPlugin);
                enMenuTrayTotp.Image = Properties.Resources.TOTP;

                var enMenuCopyTotp = new ToolStripMenuItem(Localization.Strings.CopyTOTP);
                enMenuCopyTotp.Image = Properties.Resources.TOTP;
                enMenuCopyTotp.ShortcutKeys = (Keys)Shortcut.CtrlT;
                enMenuCopyTotp.Click += OnEntryMenuTOTPClick;
                var enMenuSetupTotp = new ToolStripMenuItem(Localization.Strings.SetupTOTP);
                enMenuSetupTotp.Image = Properties.Resources.TOTP_Setup;
                enMenuSetupTotp.ShortcutKeys = (Keys)Shortcut.CtrlShiftI;
                enMenuSetupTotp.Click += OnEntryMenuSetupClick;
                var enMenuShowQr = new ToolStripMenuItem(Localization.Strings.ShowQR);
                enMenuShowQr.Image = Properties.Resources.TOTP_Setup;
                enMenuShowQr.ShortcutKeys = (Keys)Shortcut.CtrlShiftJ;
                enMenuShowQr.Click += OnEntryMenuShowQRClick;

                enMenuTrayTotp.DropDownItems.Add(enMenuCopyTotp);
                enMenuTrayTotp.DropDownItems.Add(enMenuSetupTotp);
                enMenuTrayTotp.DropDownItems.Add(enMenuShowQr);

                enMenuTrayTotp.DropDownOpening += delegate (object sender, EventArgs e)
                {
                    enMenuCopyTotp.Enabled = false;
                    enMenuSetupTotp.Enabled = false;

                    bool boolCopy = PluginHost.CustomConfig.GetBool(setname_bool_EntryContextCopy_Visible, true);
                    enMenuCopyTotp.Visible = boolCopy;

                    if (PluginHost.MainWindow.GetSelectedEntriesCount() == 1)
                    {
                        var currentEntry = PluginHost.MainWindow.GetSelectedEntry(true);
                        if (SettingsCheck(currentEntry) && SeedCheck(currentEntry))
                        {
                            if (SettingsValidate(currentEntry))
                            {
                                enMenuCopyTotp.Enabled = true;
                                enMenuCopyTotp.Tag = currentEntry;
                            }
                        }

                        enMenuSetupTotp.Enabled = true;
                    }

                    bool boolSetup = PluginHost.CustomConfig.GetBool(setname_bool_EntryContextSetup_Visible, true);
                    enMenuSetupTotp.Visible = boolSetup;
                };

                enMenuTrayTotp.DropDownClosed += delegate (object sender, EventArgs e)
                {
                    enMenuCopyTotp.Enabled = true;
                };

                return enMenuTrayTotp;
            }

            return null; // No menu items in other locations
        }

        /// <summary>
        /// Initialization of the plugin, adding menus, handlers and forms.
        /// </summary>
        /// <param name="host">Plugin host for access to KeePass functions.</param>
        /// <returns>Successful loading of the plugin, if not the plugin is removed.</returns>
        public override bool Initialize(IPluginHost host)
        {
            // Internalize Host Handle.
            if (host == null) return false;
            PluginHost = host;

            // Instantiate Help Form.
            _helpForm = new FormHelp(this);

            // Register events.
            PluginHost.MainWindow.Shown += MainWindow_Shown;

            // Notify Icon Context Menus.
            PluginHost.MainWindow.TrayContextMenu.Opening += OnNotifyMenuOpening;
            _niMenuTitle = new ToolStripMenuItem(Localization.Strings.TrayTOTPPlugin);
            _niMenuTitle.Image = Properties.Resources.TOTP;
            PluginHost.MainWindow.TrayContextMenu.Items.Insert(0, _niMenuTitle);
            _niMenuSeperator = new ToolStripSeparator();
            PluginHost.MainWindow.TrayContextMenu.Items.Insert(1, _niMenuSeperator);

            PluginHost.MainWindow.TrayContextMenu.Opened += OnTrayContextMenuOpened;

            // Register auto-type function.
            if (PluginHost.CustomConfig.GetBool(setname_bool_AutoType_Enable, true))
            {
                SprEngine.FilterCompile += SprEngine_FilterCompile;
                SprEngine.FilterPlaceholderHints.Add(PluginHost.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets());
            }

            // List Column TOTP.
            _liColumnTotp = new TrayTOTP_CustomColumn(this);
            PluginHost.ColumnProviderPool.Add(_liColumnTotp);

            // Refresh Timer.
            _liRefreshTimer.Interval = setstat_int_EntryList_RefreshRate;
            _liRefreshTimer.Enabled = true;
            _liRefreshTimer.Tick += OnTimerTick;

            //Time Correction.
            TimeCorrections = new TimeCorrectionCollection(PluginHost.CustomConfig.GetBool(setname_bool_TimeCorrection_Enable, false));
            TimeCorrectionProvider.Interval = Convert.ToInt16(PluginHost.CustomConfig.GetULong(KeeTrayTOTPExt.setname_ulong_TimeCorrection_RefreshTime, KeeTrayTOTPExt.setdef_ulong_TimeCorrection_RefreshTime));
            TimeCorrections.AddRangeFromList(PluginHost.CustomConfig.GetString(setname_string_TimeCorrection_List, string.Empty).Split(';').ToList());

            return true;
        }

        private void OnTrayContextMenuOpened(object sender, EventArgs e)
        {
            var contextMenuStrip = (ContextMenuStrip)sender;
            var dropDownLocationCalculator = new DropDownLocationCalculator(contextMenuStrip.Size);
            contextMenuStrip.Location = dropDownLocationCalculator.CalculateLocationForDropDown(Cursor.Position);
        }

        /// <summary>
        /// Occurs when the main window is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (!PluginHost.CustomConfig.GetBool(setname_bool_FirstInstall_Shown, false))
            {
                PluginHost.CustomConfig.SetBool(setname_bool_FirstInstall_Shown, true);
                if (!_helpForm.Visible)
                {
                    _helpForm = new FormHelp(this, true);
                    _helpForm.Show();
                }
                else
                {
                    _helpForm.Focus();
                }
            }
        }

        /// <summary>
        /// Tools Menu Tray TOTP Settings Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuSettingsClick(object sender, EventArgs e)
        {
            var formSettings = new FormSettings(this);
            UIUtil.ShowDialogAndDestroy(formSettings);
        }

        /// <summary>
        /// Tools Menu Tray TOTP Help Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuHelpClick(object sender, EventArgs e)
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
        /// Tools Menu Tray TOTP About Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuAboutClick(object sender, EventArgs e)
        {
            var formAbout = new FormAbout(this);
            UIUtil.ShowDialogAndDestroy(formAbout);
        }

        /// <summary>
        /// Entry Context Menu Copy Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryMenuTOTPClick(object sender, EventArgs e)
        {
            PwEntry pe = PluginHost.MainWindow.GetSelectedEntry(false);

            if (pe != null)
            {
                TOTPCopyToClipboard(pe);
            }
        }

        /// <summary>
        /// Entry Context Menu Setup Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryMenuSetupClick(object sender, EventArgs e)
        {
            if (PluginHost.MainWindow.GetSelectedEntriesCount() == 1)
            {
                var formWizard = new SetupTOTP(this, PluginHost.MainWindow.GetSelectedEntry(true));
                formWizard.ShowDialog();
                PluginHost.MainWindow.RefreshEntriesList();
            }
        }

        /// <summary>
        /// Entry Context Menu Show QR Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryMenuShowQRClick(object sender, EventArgs e)
        {
            if (PluginHost.MainWindow.GetSelectedEntriesCount() != 1) return;
            var entry = PluginHost.MainWindow.GetSelectedEntry(true);

            if (!SeedCheck(entry))
            {
                return;
            }

            var rawSeed = this.SeedGet(entry).ReadString();
            var cleanSeed = Regex.Replace(rawSeed, @"\s+", "");

            var showQr = new ShowQR
            {
                Seed = cleanSeed,
                IssuerText = { Text = entry.Strings.Get("Title").ReadString() },
                UsernameText = { Text = entry.Strings.Get("UserName").ReadString() },
            };
            showQr.ShowDialog();

            PluginHost.MainWindow.RefreshEntriesList();
        }

        /// <summary>
        /// Notify Icon Context Menu Opening.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNotifyMenuOpening(object sender, CancelEventArgs e)
        {
            foreach (var menu in _niMenuList)
            {
                PluginHost.MainWindow.TrayContextMenu.Items.Remove(menu);
            }
            _niMenuList.Clear();
            if (PluginHost.CustomConfig.GetBool(setname_bool_NotifyContext_Visible, true))
            {
                _niMenuTitle.Visible = true;
                _niMenuSeperator.Visible = true;
                if (PluginHost.MainWindow.ActiveDatabase.IsOpen)
                {
                    var trimTrayText = PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);
                    foreach (PwEntry entry in GetVisibleAndValidPasswordEntries())
                    {
                        var entryTitle = entry.Strings.ReadSafe(PwDefs.TitleField);

                        var context = new SprContext(entry, PluginHost.MainWindow.ActiveDatabase, SprCompileFlags.All, false, false);
                        var entryUsername = SprEngine.Compile(entry.Strings.ReadSafe(PwDefs.UserNameField), context);
                        string trayTitle;
                        if ((trimTrayText && entryTitle.Length + entryUsername.Length > setstat_trim_text_length) || (string.IsNullOrEmpty(entryUsername)))
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
                        if (!SettingsValidate(entry))
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
                        var newMenu = new ToolStripMenuItem(Localization.Strings.NoTOTPSeedFound);
                        newMenu.Image = Properties.Resources.TOTP_None;
                        _niMenuList.Add(newMenu);
                        PluginHost.MainWindow.TrayContextMenu.Items.Insert(1, _niMenuList[0]);
                    }

                    CreateMenuItemForOtherDatabases(_niMenuList);
                }
                else
                {
                    if (PluginHost.MainWindow.IsFileLocked(null))
                    {
                        var newMenu = new ToolStripMenuItem(Localization.Strings.DatabaseIsLocked);
                        newMenu.Image = Properties.Resources.TOTP_Lock;
                        _niMenuList.Add(newMenu);
                        PluginHost.MainWindow.TrayContextMenu.Items.Insert(1, _niMenuList[0]);
                    }
                    else
                    {
                        var newMenu = new ToolStripMenuItem(Localization.Strings.DatabaseIsNotOpen);
                        newMenu.Image = Properties.Resources.TOTP_Error;
                        _niMenuList.Add(newMenu);
                        PluginHost.MainWindow.TrayContextMenu.Items.Insert(1, _niMenuList[0]);
                    }
                }
            }
            else
            {
                _niMenuTitle.Visible = false;
                _niMenuSeperator.Visible = false;
            }
        }

        /// <summary>
        /// Get all the password entries in all groups and filter entries that are expired or have invalid TOTP settings.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<PwEntry> GetVisibleAndValidPasswordEntries()
        {
            var entries = PluginHost.MainWindow.ActiveDatabase.RootGroup.GetEntries(true);

            return entries
                .Where(entry => !entry.IsExpired())
                .Where(entry => SettingsCheck(entry) && SeedCheck(entry));
        }

        /// <summary>
        /// Creates the necessary menu items to switch to another database
        /// </summary>
        /// <param name="items"></param>
        private void CreateMenuItemForOtherDatabases(IList<ToolStripMenuItem> items)
        {
            var tabcontrol = PluginHost.MainWindow.Controls.OfType<TabControl>().FirstOrDefault();
            var nonSelectedTabs = tabcontrol.TabPages.OfType<TabPage>().Where(c => c != tabcontrol.SelectedTab).ToList();

            int i = 1;
            foreach (var tab in nonSelectedTabs)
            {
                var item = new ToolStripMenuItem(string.Format(Strings.SwitchTo, tab.Text))
                {
                    Tag = tab
                };
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

        /// <summary>
        /// Entry Notify Menu Copy Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNotifyMenuTOTPClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = sender as ToolStripMenuItem;
            if (tsi == null)
                return;

            PwEntry pe = tsi.Tag as PwEntry;

            if (pe != null)
                TOTPCopyToClipboard(pe);
        }

        /// <summary>
        /// Timer Event that occurs to refresh the entry list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if ((PluginHost.MainWindow.ActiveDatabase.IsOpen) && (PluginHost.MainWindow.Visible))
            {
                if (KeePass.Program.Config.MainWindow.EntryListColumns.Count != _liColumnsCount)
                {
                    _liColumnTotpVisible = false;
                    _liColumnsCount = KeePass.Program.Config.MainWindow.EntryListColumns.Count;
                    foreach (var column in KeePass.Program.Config.MainWindow.EntryListColumns)
                    {
                        if (column.Type == AceColumnType.PluginExt)
                        {
                            if (column.CustomName == Localization.Strings.TOTP)
                            {
                                _liColumnTotpVisible = true;
                            }
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
                            if (SettingsCheck(entry) && SeedCheck(entry))
                            {
                                _liColumnTotpContains = true;
                            }
                        }
                    }
                }

                if ((_liColumnTotpVisible) && (_liColumnTotpContains)) //Tests if displayed entries have totps that require refreshing.
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
            if ((e.Context.Flags & SprCompileFlags.ExtActive) == SprCompileFlags.ExtActive)
            {
                if (e.Text.IndexOf(PluginHost.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    if (SettingsCheck(e.Context.Entry) && SeedCheck(e.Context.Entry))
                    {
                        if (SettingsValidate(e.Context.Entry))
                        {
                            string[] settings = SettingsGet(e.Context.Entry);

                            TOTPProvider totpGenerator = new TOTPProvider(settings, ref this.TimeCorrections);

                            string invalidCharacters;

                            if (SeedValidate(e.Context.Entry, out invalidCharacters))
                            {
                                e.Context.Entry.Touch(false);
                                string totp = totpGenerator.GenerateByByte(Base32.Decode(SeedGet(e.Context.Entry).ReadString().ExtWithoutSpaces()));
                                e.Text = StrUtil.ReplaceCaseInsensitive(e.Text, PluginHost.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets(), totp);
                            }
                            else
                            {
                                e.Text = string.Empty;
                                MessageService.ShowWarning(Localization.Strings.ErrorBadSeed + invalidCharacters.ExtWithParenthesis().ExtWithSpaceBefore());
                            }
                            if (totpGenerator.TimeCorrectionError) MessageService.ShowWarning(Localization.Strings.WarningBadURL);
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
                        MessageService.ShowWarning(Localization.Strings.ErrorBadSettings);
                    }
                }
            }
        }

        /// <summary>
        /// Check if specified Entry contains Settings that are not null.
        /// </summary>
        /// <param name="pe">Pasword Entry.</param>
        /// <returns>Presence of Settings.</returns>
        internal bool SettingsCheck(PwEntry pe)
        {
            return pe.Strings.Exists(PluginHost.CustomConfig.GetString(setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings));
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry pe)
        {
            bool validInterval;
            bool validLength;
            bool validUrl; //Dummies
            return SettingsValidate(pe, out validInterval, out validLength, out validUrl);
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid. The URL status is available as an out boolean.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <param name="isUrlValid">Url Validity.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry pe, out bool isUrlValid)
        {
            bool validInterval; bool validLength; //Dummies
            return SettingsValidate(pe, out validInterval, out validLength, out isUrlValid);
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid. All settings statuses are available as out booleans.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <param name="isIntervalValid">Interval Validity.</param>
        /// <param name="isLengthValid">Length Validity.</param>
        /// <param name="isUrlValid">Url Validity.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry pe, out bool isIntervalValid, out bool isLengthValid, out bool isUrlValid)
        {
            bool settingsValid;
            try
            {
                string[] settings = SettingsGet(pe);

                isIntervalValid = IntervalIsValid(settings);
                isLengthValid = LengthIsValid(settings);

                settingsValid = isIntervalValid && isLengthValid;

                isUrlValid = UrlIsValid(settings);
            }
            catch (Exception)
            {
                isIntervalValid = false;
                isLengthValid = false;
                isUrlValid = false;
                settingsValid = false;
            }
            return settingsValid;
        }

        private static bool UrlIsValid(string[] settings)
        {
            if (settings.Length < 3)
                return false;

            return settings[2].StartsWith("http://") || settings[2].StartsWith("https://");
        }

        private bool LengthIsValid(string[] settings)
        {
            if (settings.Length < 2)
                return false;

            if (!setstat_allowed_lengths.Contains(settings[1]))
                return false;

            return true;
        }

        private bool IntervalIsValid(string[] settings)
        {
            if (settings.Length == 0)
                return false;

            short interval;
            if (!short.TryParse(settings[0], out interval))
                return false;

            if (interval < 0 && interval < 180)
                return false;

            return true;
        }

        /// <summary>
        /// Get the entry's Settings using the string name specified in the settings (or default).
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>String Array (Interval, Length, Url).</returns>
        internal string[] SettingsGet(PwEntry pe)
        {
            return pe.Strings.Get(PluginHost.CustomConfig.GetString(setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings)).ReadString().Split(';');
        }

        /// <summary>
        /// Check if the specified Entry contains a Seed.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Presence of the Seed.</returns>
        internal bool SeedCheck(PwEntry pe)
        {
            return pe.Strings.Exists(PluginHost.CustomConfig.GetString(setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed));
        }

        /// <summary>
        /// Validates the entry's Seed making sure it's a valid Base32 string.
        /// </summary>
        /// <param name="passwordEntry">Password Entry.</param>
        /// <returns>Validity of the Seed's characters for Base32 format.</returns>
        internal bool SeedValidate(PwEntry passwordEntry)
        {
            string invalidCharacters;
            return SeedGet(passwordEntry).ReadString().ExtWithoutSpaces().ExtIsBase32(out invalidCharacters);
        }

        /// <summary>
        /// Validates the entry's Seed making sure it's a valid Base32 string. Invalid characters are available as out string.
        /// </summary>
        /// <param name="passwordEntry">Password Entry.</param>
        /// <param name="invalidChars">Password Entry.</param>
        /// <returns>Validity of the Seed's characters.</returns>
        internal bool SeedValidate(PwEntry passwordEntry, out string invalidChars)
        {
            return SeedGet(passwordEntry).ReadString().ExtWithoutSpaces().ExtIsBase32(out invalidChars);
        }

        /// <summary>
        /// Get the entry's Seed using the string name specified in the settings (or default).
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Protected Seed.</returns>
        internal ProtectedString SeedGet(PwEntry pe)
        {
            return pe.Strings.Get(PluginHost.CustomConfig.GetString(setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed));
        }

        /// <summary>
        /// Copies the specified entry's generated TOTP to the clipboard using the KeePass's clipboard function.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        private void TOTPCopyToClipboard(PwEntry pe)
        {
            if (SettingsCheck(pe) && SeedCheck(pe))
            {
                if (SettingsValidate(pe))
                {
                    string[] settings = SettingsGet(pe);

                    TOTPProvider totpGenerator = new TOTPProvider(settings, ref this.TimeCorrections);

                    string invalidCharacters;
                    if (SeedValidate(pe, out invalidCharacters))
                    {
                        pe.Touch(false);

                        string totp = totpGenerator.Generate(SeedGet(pe).ReadString().ExtWithoutSpaces());

                        ClipboardUtil.CopyAndMinimize(totp, true, PluginHost.MainWindow, pe, PluginHost.MainWindow.ActiveDatabase);
                        PluginHost.MainWindow.StartClipboardCountdown();
                    }
                    else
                    {
                        MessageService.ShowWarning(Localization.Strings.ErrorBadSeed + invalidCharacters.ExtWithParenthesis().ExtWithSpaceBefore());
                    }
                    if (totpGenerator.TimeCorrectionError) MessageService.ShowWarning(Localization.Strings.WarningBadURL);
                }
                else
                {
                    MessageService.ShowWarning(Localization.Strings.ErrorBadSettings);
                }
            }
            else
            {
                MessageService.ShowWarning(Localization.Strings.ErrorBadSettings);
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
            PluginHost.MainWindow.TrayContextMenu.Items.Remove(_niMenuSeperator);
            _niMenuSeperator.Dispose();

            // Unregister auto-type function.
            if (SprEngine.FilterPlaceholderHints.Contains(PluginHost.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets()))
            {
                SprEngine.FilterCompile -= SprEngine_FilterCompile;
                SprEngine.FilterPlaceholderHints.Remove(PluginHost.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets());
            }

            // Remove Column provider.
            PluginHost.ColumnProviderPool.Remove(_liColumnTotp);
            _liColumnTotp = null;

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
            get { return "https://raw.githubusercontent.com/victor-rds/KeeTrayTOTP/master/version_manifest.txt"; }
        }
    }
}
