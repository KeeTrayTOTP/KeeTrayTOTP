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
        internal IPluginHost m_host = null;

        /// <summary>
        /// Tray TOTP Support Email
        /// </summary>
        internal const string strEmail = "victor.rds@protonmail.ch";

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
        private FormHelp HelpForm;

        /// <summary>
        /// Notify Icon Context Menu Title.
        /// </summary>
        private ToolStripMenuItem niMenuTitle;

        /// <summary>
        /// Notify Icon Context Menu List.
        /// </summary>
        private List<ToolStripMenuItem> niMenuList = new List<ToolStripMenuItem>();
        /// <summary>
        /// Notify Icon Context Menu Separator.
        /// </summary>
        private ToolStripSeparator niMenuSeperator = null;

        /// <summary>
        /// Entries Column TOTP.
        /// </summary>
        private TrayTOTP_CustomColumn liColumnTOTP = null;

        /// <summary>
        /// Entry List Column Count.
        /// </summary>
        private int liColumnsCount = 0;
        /// <summary>
        /// Entry List Column TOTP visibility.
        /// </summary>
        private bool liColumnTOTPVisible = false;
        /// <summary>
        /// Entry Groups last selected group.
        /// </summary>
        private PwGroup liGroupsPreviousSelected = null;
        /// <summary>
        /// Entry Column TOTP has TOTPs.
        /// </summary>
        private bool liColumnTOTPContains = false;

        /// <summary>
        /// Entries Refresh Timer.
        /// </summary>
        private Timer liRefreshTimer = new Timer();

        /// <summary>
        /// Entries Refresh Timer.
        /// </summary>
        internal int liRefreshTimerInterval
        {
            set
            {
                liRefreshTimer.Interval = value;
                liRefreshTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Entries Refresh Timer Previous Counter to Prevent Useless Refresh.
        /// </summary>
        private int liRefreshTimerPreviousCounter;

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
                var enMenuTrayTOTP = new ToolStripMenuItem(Localization.Strings.TrayTOTPPlugin);
                enMenuTrayTOTP.Image = Properties.Resources.TOTP;

                var enMenuCopyTOTP = new ToolStripMenuItem(Localization.Strings.CopyTOTP);
                enMenuCopyTOTP.Image = Properties.Resources.TOTP;
                enMenuCopyTOTP.ShortcutKeys = (Keys)Shortcut.CtrlT;
                enMenuCopyTOTP.Click += OnEntryMenuTOTPClick;
                var enMenuSetupTOTP = new ToolStripMenuItem(Localization.Strings.SetupTOTP);
                enMenuSetupTOTP.Image = Properties.Resources.TOTP_Setup;
                enMenuSetupTOTP.ShortcutKeys = (Keys)Shortcut.CtrlShiftI;
                enMenuSetupTOTP.Click += OnEntryMenuSetupClick;
                var enMenuShowQR = new ToolStripMenuItem(Localization.Strings.ShowQR);
                enMenuShowQR.Image = Properties.Resources.TOTP_Setup;
                enMenuShowQR.ShortcutKeys = (Keys)Shortcut.CtrlShiftJ;
                enMenuShowQR.Click += OnEntryMenuShowQRClick;

                enMenuTrayTOTP.DropDownItems.Add(enMenuCopyTOTP);
                enMenuTrayTOTP.DropDownItems.Add(enMenuSetupTOTP);
                enMenuTrayTOTP.DropDownItems.Add(enMenuShowQR);

                enMenuTrayTOTP.DropDownOpening += delegate (object sender, EventArgs e)
                {
                    enMenuCopyTOTP.Enabled = false;
                    enMenuSetupTOTP.Enabled = false;

                    bool boolCopy = m_host.CustomConfig.GetBool(setname_bool_EntryContextCopy_Visible, true);
                    enMenuCopyTOTP.Visible = boolCopy;

                    if (m_host.MainWindow.GetSelectedEntriesCount() == 1)
                    {
                        var CurrentEntry = m_host.MainWindow.GetSelectedEntry(true);
                        if (SettingsCheck(CurrentEntry) && SeedCheck(CurrentEntry))
                        {
                            if (SettingsValidate(CurrentEntry))
                            {
                                enMenuCopyTOTP.Enabled = true;
                                enMenuCopyTOTP.Tag = CurrentEntry;
                            }
                        }

                        enMenuSetupTOTP.Enabled = true;
                    }

                    bool boolSetup = m_host.CustomConfig.GetBool(setname_bool_EntryContextSetup_Visible, true);
                    enMenuSetupTOTP.Visible = boolSetup;
                };

                enMenuTrayTOTP.DropDownClosed += delegate (object sender, EventArgs e)
                {
                    enMenuCopyTOTP.Enabled = true;
                };

                return enMenuTrayTOTP;
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
            m_host = host;

            // Instantiate Help Form.
            HelpForm = new FormHelp(this);

            // Register events.
            m_host.MainWindow.Shown += MainWindow_Shown;

            // Notify Icon Context Menus.
            m_host.MainWindow.TrayContextMenu.Opening += OnNotifyMenuOpening;
            niMenuTitle = new ToolStripMenuItem(Localization.Strings.TrayTOTPPlugin);
            niMenuTitle.Image = Properties.Resources.TOTP;
            m_host.MainWindow.TrayContextMenu.Items.Insert(0, niMenuTitle);
            niMenuSeperator = new ToolStripSeparator();
            m_host.MainWindow.TrayContextMenu.Items.Insert(1, niMenuSeperator);

            // Register auto-type function.
            if (m_host.CustomConfig.GetBool(setname_bool_AutoType_Enable, true))
            {
                SprEngine.FilterCompile += SprEngine_FilterCompile;
                SprEngine.FilterPlaceholderHints.Add(m_host.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets());
            }

            // List Column TOTP.
            liColumnTOTP = new TrayTOTP_CustomColumn(this);
            m_host.ColumnProviderPool.Add(liColumnTOTP);

            // Refresh Timer.
            liRefreshTimer.Interval = setstat_int_EntryList_RefreshRate;
            liRefreshTimer.Enabled = true;
            liRefreshTimer.Tick += OnTimerTick;

            //Time Correction.
            TimeCorrections = new TimeCorrectionCollection(m_host.CustomConfig.GetBool(setname_bool_TimeCorrection_Enable, false));
            TimeCorrectionProvider.Interval = Convert.ToInt16(m_host.CustomConfig.GetULong(KeeTrayTOTPExt.setname_ulong_TimeCorrection_RefreshTime, KeeTrayTOTPExt.setdef_ulong_TimeCorrection_RefreshTime));
            TimeCorrections.AddRangeFromList(m_host.CustomConfig.GetString(setname_string_TimeCorrection_List, string.Empty).Split(';').ToList());

            return true;
        }

        /// <summary>
        /// Occurs when the main window is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (!m_host.CustomConfig.GetBool(setname_bool_FirstInstall_Shown, false))
            {
                m_host.CustomConfig.SetBool(setname_bool_FirstInstall_Shown, true);
                if (!HelpForm.Visible)
                {
                    HelpForm = new FormHelp(this, true);
                    HelpForm.Show();
                }
                else
                {
                    HelpForm.Focus();
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
            var FormSettings = new FormSettings(this);
            UIUtil.ShowDialogAndDestroy(FormSettings);
        }

        /// <summary>
        /// Tools Menu Tray TOTP Help Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuHelpClick(object sender, EventArgs e)
        {
            if (!HelpForm.Visible)
            {
                HelpForm.Show();
            }
            else
            {
                HelpForm.Focus();
            }
        }

        /// <summary>
        /// Tools Menu Tray TOTP About Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuAboutClick(object sender, EventArgs e)
        {
            var FormAbout = new FormAbout(this);
            UIUtil.ShowDialogAndDestroy(FormAbout);
        }

        /// <summary>
        /// Entry Context Menu Copy Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryMenuTOTPClick(object sender, EventArgs e)
        {
            PwEntry pe = m_host.MainWindow.GetSelectedEntry(false);

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
            if (m_host.MainWindow.GetSelectedEntriesCount() == 1)
            {
                var FormWizard = new SetupTOTP(this, m_host.MainWindow.GetSelectedEntry(true));
                FormWizard.ShowDialog();
                m_host.MainWindow.RefreshEntriesList();
            }
        }

        /// <summary>
        /// Entry Context Menu Show QR Click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryMenuShowQRClick(object sender, EventArgs e)
        {
            if (m_host.MainWindow.GetSelectedEntriesCount() != 1) return;
            var entry = m_host.MainWindow.GetSelectedEntry(true);

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

            m_host.MainWindow.RefreshEntriesList();
        }

        /// <summary>
        /// Notify Icon Context Menu Opening.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNotifyMenuOpening(object sender, CancelEventArgs e)
        {
            foreach (var Menu in niMenuList)
            {
                m_host.MainWindow.TrayContextMenu.Items.Remove(Menu);
            }
            niMenuList.Clear();
            if (m_host.CustomConfig.GetBool(setname_bool_NotifyContext_Visible, true))
            {
                niMenuTitle.Visible = true;
                niMenuSeperator.Visible = true;
                if (m_host.MainWindow.ActiveDatabase.IsOpen)
                {
                    var trimTrayText = m_host.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);
                    foreach (PwEntry Entry in m_host.MainWindow.ActiveDatabase.RootGroup.GetEntries(true))
                    {
                        if (SettingsCheck(Entry) && SeedCheck(Entry))
                        {
                            var entryTitle = Entry.Strings.ReadSafe(PwDefs.TitleField);

                            var context = new SprContext(Entry, m_host.MainWindow.ActiveDatabase, SprCompileFlags.All, false, false);
                            var entryUsername = SprEngine.Compile(Entry.Strings.ReadSafe(PwDefs.UserNameField), context);
                            string trayTitle;
                            if (trimTrayText && entryTitle.Length + entryUsername.Length > setstat_trim_text_length)
                            {
                                trayTitle = entryTitle.ExtWithSpaceAfter();
                            }
                            else
                            {
                                trayTitle = entryTitle.ExtWithSpaceAfter() + entryUsername.ExtWithParenthesis();
                            }
                            m_host.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);

                            var NewMenu = new ToolStripMenuItem(trayTitle, Properties.Resources.TOTP_Key, OnNotifyMenuTOTPClick);
                            NewMenu.Tag = Entry;
                            if (!SettingsValidate(Entry))
                            {
                                NewMenu.Enabled = false;
                                NewMenu.Image = Properties.Resources.TOTP_Error;
                            }
                            niMenuList.Add(NewMenu);
                        }
                    }
                    if (niMenuList.Count > 0)
                    {
                        niMenuList.Sort((p1, p2) => string.Compare(p1.Text, p2.Text, true));
                        for (int i = 0; i <= niMenuList.Count - 1; i++)
                        {
                            m_host.MainWindow.TrayContextMenu.Items.Insert(i + 1, niMenuList[i]);
                        }
                    }
                    else
                    {
                        var NewMenu = new ToolStripMenuItem(Localization.Strings.NoTOTPSeedFound);
                        NewMenu.Image = Properties.Resources.TOTP_None;
                        niMenuList.Add(NewMenu);
                        m_host.MainWindow.TrayContextMenu.Items.Insert(1, niMenuList[0]);
                    }

                    CreateMenuItemForOtherDatabases(niMenuList);
                }
                else
                {
                    if (m_host.MainWindow.IsFileLocked(null))
                    {
                        var NewMenu = new ToolStripMenuItem(Localization.Strings.DatabaseIsLocked);
                        NewMenu.Image = Properties.Resources.TOTP_Lock;
                        niMenuList.Add(NewMenu);
                        m_host.MainWindow.TrayContextMenu.Items.Insert(1, niMenuList[0]);
                    }
                    else
                    {
                        var NewMenu = new ToolStripMenuItem(Localization.Strings.DatabaseIsNotOpen);
                        NewMenu.Image = Properties.Resources.TOTP_Error;
                        niMenuList.Add(NewMenu);
                        m_host.MainWindow.TrayContextMenu.Items.Insert(1, niMenuList[0]);
                    }
                }
            }
            else
            {
                niMenuTitle.Visible = false;
                niMenuSeperator.Visible = false;
            }
        }

        /// <summary>
        /// Creates the necessary menu items to switch to another database
        /// </summary>
        /// <param name="items"></param>
        private void CreateMenuItemForOtherDatabases(IList<ToolStripMenuItem> items)
        {
            var tabcontrol = m_host.MainWindow.Controls.OfType<TabControl>().FirstOrDefault();
            var nonSelectedTabs = tabcontrol.TabPages.OfType<TabPage>().Where(c => c != tabcontrol.SelectedTab).ToList();

            int i = 1;
            foreach (var tab in nonSelectedTabs)
            {
                var item = new ToolStripMenuItem("Switch to " + tab.Text)
                {
                    Tag = tab
                };
                item.Click += SwitchToOtherDatabase;
                items.Add(item);
                
                m_host.MainWindow.TrayContextMenu.Items.Insert(i++, item);
            }
        }

        private void SwitchToOtherDatabase(object sender, EventArgs e)
        {
            var tabControl = m_host.MainWindow.Controls.OfType<TabControl>().FirstOrDefault();
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
            if ((m_host.MainWindow.ActiveDatabase.IsOpen) && (m_host.MainWindow.Visible))
            {
                if (KeePass.Program.Config.MainWindow.EntryListColumns.Count != liColumnsCount)
                {
                    liColumnTOTPVisible = false;
                    liColumnsCount = KeePass.Program.Config.MainWindow.EntryListColumns.Count;
                    foreach (var Column in KeePass.Program.Config.MainWindow.EntryListColumns)
                    {
                        if (Column.Type == AceColumnType.PluginExt)
                        {
                            if (Column.CustomName == Localization.Strings.TOTP)
                            {
                                liColumnTOTPVisible = true;
                            }
                        }
                    }
                }

                if (liColumnTOTPVisible)
                {
                    PwGroup SelectedGroup = m_host.MainWindow.GetSelectedGroup();
                    if (SelectedGroup != liGroupsPreviousSelected)
                    {
                        liColumnTOTPContains = false;
                        liGroupsPreviousSelected = SelectedGroup;
                        foreach (var Entry in SelectedGroup.GetEntries(true))
                        {
                            if (SettingsCheck(Entry) && SeedCheck(Entry))
                            {
                                liColumnTOTPContains = true;
                            }
                        }
                    }
                }

                if ((liColumnTOTPVisible) && (liColumnTOTPContains)) //Tests if displayed entries have totps that require refreshing.
                {
                    var CurrentSeconds = DateTime.Now.Second;
                    if (liRefreshTimerPreviousCounter != CurrentSeconds)
                    {
                        m_host.MainWindow.RefreshEntriesList();
                        liRefreshTimerPreviousCounter = CurrentSeconds;
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
                if (e.Text.IndexOf(m_host.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    if (SettingsCheck(e.Context.Entry) && SeedCheck(e.Context.Entry))
                    {
                        if (SettingsValidate(e.Context.Entry))
                        {
                            string[] Settings = SettingsGet(e.Context.Entry);

                            TOTPProvider TOTPGenerator = new TOTPProvider(Settings, ref this.TimeCorrections);

                            string InvalidCharacters;

                            if (SeedValidate(e.Context.Entry, out InvalidCharacters))
                            {
                                e.Context.Entry.Touch(false);
                                string totp = TOTPGenerator.GenerateByByte(Base32.Decode(SeedGet(e.Context.Entry).ReadString().ExtWithoutSpaces()));
                                e.Text = StrUtil.ReplaceCaseInsensitive(e.Text, m_host.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets(), totp);
                            }
                            else
                            {
                                e.Text = string.Empty;
                                MessageService.ShowWarning(Localization.Strings.ErrorBadSeed + InvalidCharacters.ExtWithParenthesis().ExtWithSpaceBefore());
                            }
                            if (TOTPGenerator.TimeCorrectionError) MessageService.ShowWarning(Localization.Strings.WarningBadURL);
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
            return pe.Strings.Exists(m_host.CustomConfig.GetString(setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings));
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry pe)
        {
            bool ValidInterval;
            bool ValidLength;
            bool ValidUrl; //Dummies
            return SettingsValidate(pe, out ValidInterval, out ValidLength, out ValidUrl);
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid. The URL status is available as an out boolean.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <param name="IsUrlValid">Url Validity.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry pe, out bool IsUrlValid)
        {
            bool ValidInterval; bool ValidLength; //Dummies
            return SettingsValidate(pe, out ValidInterval, out ValidLength, out IsUrlValid);
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid. All settings statuses are available as out booleans.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <param name="IsIntervalValid">Interval Validity.</param>
        /// <param name="IsLengthValid">Length Validity.</param>
        /// <param name="IsUrlValid">Url Validity.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry pe, out bool IsIntervalValid, out bool IsLengthValid, out bool IsUrlValid)
        {
            bool settingsValid = true;
            try
            {
                string[] settings = SettingsGet(pe);

                IsIntervalValid = IntervalIsValid(settings);
                IsLengthValid = LengthIsValid(settings);

                settingsValid = IsIntervalValid && IsLengthValid;

                IsUrlValid = UrlIsValid(settings);
            }
            catch (Exception)
            {
                IsIntervalValid = false;
                IsLengthValid = false;
                IsUrlValid = false;
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
            return pe.Strings.Get(m_host.CustomConfig.GetString(setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings)).ReadString().Split(';');
        }

        /// <summary>
        /// Check if the specified Entry contains a Seed.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Presence of the Seed.</returns>
        internal bool SeedCheck(PwEntry pe)
        {
            return pe.Strings.Exists(m_host.CustomConfig.GetString(setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed));
        }

        /// <summary>
        /// Validates the entry's Seed making sure it's a valid Base32 string.
        /// </summary>
        /// <param name="PasswordEntry">Password Entry.</param>
        /// <returns>Validity of the Seed's characters for Base32 format.</returns>
        internal bool SeedValidate(PwEntry PasswordEntry)
        {
            string InvalidCharacters;
            return SeedGet(PasswordEntry).ReadString().ExtWithoutSpaces().ExtIsBase32(out InvalidCharacters);
        }

        /// <summary>
        /// Validates the entry's Seed making sure it's a valid Base32 string. Invalid characters are available as out string.
        /// </summary>
        /// <param name="PasswordEntry">Password Entry.</param>
        /// <param name="InvalidChars">Password Entry.</param>
        /// <returns>Validity of the Seed's characters.</returns>
        internal bool SeedValidate(PwEntry PasswordEntry, out string InvalidChars)
        {
            return SeedGet(PasswordEntry).ReadString().ExtWithoutSpaces().ExtIsBase32(out InvalidChars);
        }

        /// <summary>
        /// Get the entry's Seed using the string name specified in the settings (or default).
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Protected Seed.</returns>
        internal ProtectedString SeedGet(PwEntry pe)
        {
            return pe.Strings.Get(m_host.CustomConfig.GetString(setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed));
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
                    string[] Settings = SettingsGet(pe);

                    TOTPProvider TOTPGenerator = new TOTPProvider(Settings, ref this.TimeCorrections);

                    string InvalidCharacters;
                    if (SeedValidate(pe, out InvalidCharacters))
                    {
                        pe.Touch(false);

                        string totp = TOTPGenerator.Generate(SeedGet(pe).ReadString().ExtWithoutSpaces());

                        ClipboardUtil.CopyAndMinimize(totp, true, m_host.MainWindow, pe, m_host.MainWindow.ActiveDatabase);
                        m_host.MainWindow.StartClipboardCountdown();
                    }
                    else
                    {
                        MessageService.ShowWarning(Localization.Strings.ErrorBadSeed + InvalidCharacters.ExtWithParenthesis().ExtWithSpaceBefore());
                    }
                    if (TOTPGenerator.TimeCorrectionError) MessageService.ShowWarning(Localization.Strings.WarningBadURL);
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
            if (HelpForm.Visible)
            {
                HelpForm.Close();
            }
            else
            {
                HelpForm.Dispose();
            }

            // Unregister internal events.
            m_host.MainWindow.Shown -= MainWindow_Shown;

            // Remove Notify Icon menus.
            m_host.MainWindow.TrayContextMenu.Opening -= OnNotifyMenuOpening;
            m_host.MainWindow.TrayContextMenu.Items.Remove(niMenuTitle);
            niMenuTitle.Dispose();
            foreach (var Menu in niMenuList)
            {
                m_host.MainWindow.TrayContextMenu.Items.Remove(Menu);
                Menu.Dispose();
            }
            m_host.MainWindow.TrayContextMenu.Items.Remove(niMenuSeperator);
            niMenuSeperator.Dispose();

            // Unregister auto-type function.
            if (SprEngine.FilterPlaceholderHints.Contains(m_host.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets()))
            {
                SprEngine.FilterCompile -= SprEngine_FilterCompile;
                SprEngine.FilterPlaceholderHints.Remove(m_host.CustomConfig.GetString(setname_string_AutoType_FieldName, setdef_string_AutoType_FieldName).ExtWithBrackets());
            }

            // Remove Column provider.
            m_host.ColumnProviderPool.Remove(liColumnTOTP);
            liColumnTOTP = null;

            // Remove Timer.
            liRefreshTimer.Tick -= OnTimerTick;
            liRefreshTimer.Dispose();
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
