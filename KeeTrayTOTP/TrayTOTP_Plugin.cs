using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
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
                var toMenuTrayTOTP = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strTrayTOTPPlugin);
                toMenuTrayTOTP.Image = Properties.Resources.TOTP;

                var toSubMenuSettings = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strSettings);
                toSubMenuSettings.Image = Properties.Resources.TOTP_Settings;
                toSubMenuSettings.Click += OnMenuSettingsClick;

                toMenuTrayTOTP.DropDownItems.Add(toSubMenuSettings);
                var toSubMenuSeparator1 = new ToolStripSeparator();
                toMenuTrayTOTP.DropDownItems.Add(toSubMenuSeparator1);
                var toSubMenuHelp = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strHelp);
                toSubMenuHelp.Image = Properties.Resources.TOTP_Help;
                toSubMenuHelp.Click += OnMenuHelpClick;
                toMenuTrayTOTP.DropDownItems.Add(toSubMenuHelp);
                var toSubMenuAbout = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strAbout + "...");
                toSubMenuAbout.Image = Properties.Resources.TOTP_Info;
                toSubMenuAbout.Click += OnMenuAboutClick;
                toMenuTrayTOTP.DropDownItems.Add(toSubMenuAbout);

                return toMenuTrayTOTP;
            }
            else if (type == PluginMenuType.Entry)
            {
                var enMenuTrayTOTP = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strTrayTOTPPlugin);
                enMenuTrayTOTP.Image = Properties.Resources.TOTP;

                var enMenuCopyTOTP = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strCopyTOTP);
                enMenuCopyTOTP.Image = Properties.Resources.TOTP;
                enMenuCopyTOTP.ShortcutKeys = (Keys)Shortcut.CtrlT;
                enMenuCopyTOTP.Click += OnEntryMenuTOTPClick;
                var enMenuSetupTOTP = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strSetupTOTP);
                enMenuSetupTOTP.Image = Properties.Resources.TOTP_Setup;
                enMenuSetupTOTP.ShortcutKeys = (Keys)Shortcut.CtrlShiftI;
                enMenuSetupTOTP.Click += OnEntryMenuSetupClick;
                var enMenuShowQR = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strShowQR);
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
            niMenuTitle = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strTrayTOTPPlugin);
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
                        var NewMenu = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strNoTOTPFound);
                        NewMenu.Image = Properties.Resources.TOTP_None;
                        niMenuList.Add(NewMenu);
                        m_host.MainWindow.TrayContextMenu.Items.Insert(1, niMenuList[0]);
                    }
                }
                else
                {
                    if (m_host.MainWindow.IsFileLocked(null))
                    {
                        var NewMenu = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strDatabaseIsLocked);
                        NewMenu.Image = Properties.Resources.TOTP_Lock;
                        niMenuList.Add(NewMenu);
                        m_host.MainWindow.TrayContextMenu.Items.Insert(1, niMenuList[0]);
                    }
                    else
                    {
                        var NewMenu = new ToolStripMenuItem(TrayTOTP_Plugin_Localization.strDatabaseNotOpen);
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
                            if (Column.CustomName == TrayTOTP_Plugin_Localization.strTOTP)
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
                        bool ValidInterval = false; bool ValidLength = false; bool ValidUrl = false;
                        if (SettingsValidate(e.Context.Entry, out ValidInterval, out ValidLength, out ValidUrl))
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
                                MessageService.ShowWarning(TrayTOTP_Plugin_Localization.strWarningBadSeed + InvalidCharacters.ExtWithParenthesis().ExtWithSpaceBefore());
                            }
                            if (TOTPGenerator.TimeCorrectionError) MessageService.ShowWarning(TrayTOTP_Plugin_Localization.strWarningBadUrl);
                        }
                        else
                        {
                            e.Text = string.Empty;
                            MessageService.ShowWarning(TrayTOTP_Plugin_Localization.strWarningBadSet);
                        }
                    }
                    else
                    {
                        e.Text = string.Empty;
                        MessageService.ShowWarning(TrayTOTP_Plugin_Localization.strWarningNotSet);
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
            return pe.Strings.Exists(m_host.CustomConfig.GetString(setname_string_TOTPSettings_StringName, TrayTOTP_Plugin_Localization.setdef_string_TOTPSettings_StringName));
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
            bool SettingsValid = true;
            try
            {
                string[] Settings = SettingsGet(pe);
                try
                {
                    IsIntervalValid = (Convert.ToInt16(Settings[0]) > 0) && (Convert.ToInt16(Settings[0]) < 61); //Interval
                }
                catch (Exception)
                {
                    IsIntervalValid = false;
                    SettingsValid = false;
                }
                try
                {
                    IsLengthValid = (Settings[1] == "6") || (Settings[1] == "8") || (Settings[1] == "S"); //Length
                }
                catch (Exception)
                {
                    IsLengthValid = false;
                    SettingsValid = false;
                }
                try
                {
                    IsUrlValid = (Settings[2].StartsWith("http://")) || (Settings[2].StartsWith("https://")); //Url
                }
                catch (Exception)
                {
                    IsUrlValid = false;
                }
            }
            catch (Exception)
            {
                IsIntervalValid = false;
                IsLengthValid = false;
                IsUrlValid = false;
                SettingsValid = false;
            }
            return SettingsValid;
        }

        /// <summary>
        /// Get the entry's Settings using the string name specified in the settings (or default).
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>String Array (Interval, Length, Url).</returns>
        internal string[] SettingsGet(PwEntry pe)
        {
            return pe.Strings.Get(m_host.CustomConfig.GetString(setname_string_TOTPSettings_StringName, TrayTOTP_Plugin_Localization.setdef_string_TOTPSettings_StringName)).ReadString().Split(';');
        }

        /// <summary>
        /// Check if the specified Entry contains a Seed.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Presence of the Seed.</returns>
        internal bool SeedCheck(PwEntry pe)
        {
            return pe.Strings.Exists(m_host.CustomConfig.GetString(setname_string_TOTPSeed_StringName, TrayTOTP_Plugin_Localization.setdef_string_TOTPSeed_StringName));
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
            return pe.Strings.Get(m_host.CustomConfig.GetString(setname_string_TOTPSeed_StringName, TrayTOTP_Plugin_Localization.setdef_string_TOTPSeed_StringName));
        }

        /// <summary>
        /// Copies the specified entry's generated TOTP to the clipboard using the KeePass's clipboard function.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        private void TOTPCopyToClipboard(PwEntry pe)
        {
            if (SettingsCheck(pe) && SeedCheck(pe))
            {
                bool ValidInterval; bool ValidLength; bool ValidUrl;
                if (SettingsValidate(pe, out ValidInterval, out ValidLength, out ValidUrl))
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
                        MessageService.ShowWarning(TrayTOTP_Plugin_Localization.strWarningBadSeed + InvalidCharacters.ExtWithParenthesis().ExtWithSpaceBefore());
                    }
                    if (TOTPGenerator.TimeCorrectionError) MessageService.ShowWarning(TrayTOTP_Plugin_Localization.strWarningBadUrl);
                }
                else
                {
                    MessageService.ShowWarning(TrayTOTP_Plugin_Localization.strWarningBadSet);
                }
            }
            else
            {
                MessageService.ShowWarning(TrayTOTP_Plugin_Localization.strWarningNotSet);
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
