using KeePass.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KeeTrayTOTP
{
    public class Settings : ISettings
    {
        private static class SettingKeys
        {
            internal const string AutoTypeEnable = "autotype_enable";
            internal const string AutoTypeFieldName = "autotype_fieldname";

            internal const string EntryContextCopyVisible = "entrycontextcopy_visible";
            internal const string EntryContextSetupVisible = "entrycontextsetup_visible";

            internal const string FirstInstallShown = "firstinstall_shown";
            internal const string NotifyContextVisible = "notifycontext_visible";

            internal const string TimeCorrectionEnable = "timecorrection_enable";
            internal const string TimeCorrectionRefreshTime = "timecorrection_refreshtime";

            internal const string TOTPSeedStringName = "totpseed_stringname";
            internal const string TOTPSettingsStringName = "totpsettings_stringname";

            internal const string TOTPColumnCopyEnable = "totpcolumncopy_enable";
            internal const string TOTPColumnTimerVisible = "totpcolumntimer_visible";

            internal const string TrimTrayText = "traytotp_trim_tray_text";

            /// <summary>
            /// Constants (custom string key).
            /// </summary>
            internal const string TimeCorrectionList = "traytotp_timecorrection_list";
        }

        private static class SettingDefaults
        {
            /// <summary>
            /// Constants (default settings values).
            /// </summary>
            internal const string AutoTypeFieldName = "TOTP";
            internal const int TimeCorrectionRefreshTime = 60;
            internal const int TrimTextLength = 25;
            internal const int EntryListRefreshRate = 300;
            internal static readonly ReadOnlyCollection<string> AllowedLengths = new ReadOnlyCollection<string>(new[] { "6", "7", "8", "S" });
        }

        private readonly IPluginHost _pluginHost;

        public Settings(IPluginHost pluginHost)
        {
            this._pluginHost = pluginHost;
        }      

        public ReadOnlyCollection<string> AllowedLengths
        {
            get { return SettingDefaults.AllowedLengths; }
        }

        public int TrimTextLength
        {
            get { return SettingDefaults.TrimTextLength; }
        }

        public int EntryListRefreshRate
        {
            get { return SettingDefaults.EntryListRefreshRate; }
        }

        public bool EntryContextCopyVisible
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.EntryContextCopyVisible, true); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.EntryContextCopyVisible, value); }
        }

        public bool EntryContextSetupVisible
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.EntryContextSetupVisible, true); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.EntryContextSetupVisible, value); }
        }

        public bool NotifyContextVisible
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.NotifyContextVisible, true); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.NotifyContextVisible, value); }
        }

        public bool TrimTrayText
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.TrimTrayText, false); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.TrimTrayText, value); }
        }

        public bool TOTPColumnCopyEnable
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.TOTPColumnCopyEnable, true); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.TOTPColumnCopyEnable, value); }
        }

        public bool TOTPColumnTimerVisible
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.TOTPColumnTimerVisible, true); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.TOTPColumnTimerVisible, value); }
        }

        public bool AutoTypeEnable
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.AutoTypeEnable, true); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.AutoTypeEnable, value); }
        }

        public string AutoTypeFieldName
        {
            get { return _pluginHost.CustomConfig.GetString(SettingKeys.AutoTypeFieldName, SettingDefaults.AutoTypeFieldName); }
            set { _pluginHost.CustomConfig.SetString(SettingKeys.AutoTypeFieldName, value); }
        }

        public bool TimeCorrectionEnable
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.TimeCorrectionEnable, false); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.TimeCorrectionEnable, value); }
        }

        public ulong TimeCorrectionRefreshTime
        {
            get { return _pluginHost.CustomConfig.GetULong(SettingKeys.TimeCorrectionRefreshTime, SettingDefaults.TimeCorrectionRefreshTime); }
            set { _pluginHost.CustomConfig.SetULong(SettingKeys.TimeCorrectionRefreshTime, value); }
        }

        public IEnumerable<string> TimeCorrectionList
        {
            get { return _pluginHost.CustomConfig.GetString(SettingKeys.TimeCorrectionList, string.Empty).Split(';'); }
            set { _pluginHost.CustomConfig.SetString(SettingKeys.TimeCorrectionList, string.Join(";", value)); }
        }

        public string TOTPSeedStringName
        {
            get { return _pluginHost.CustomConfig.GetString(SettingKeys.TOTPSeedStringName, Localization.Strings.TOTPSeed); }
            set { _pluginHost.CustomConfig.SetString(SettingKeys.TOTPSeedStringName, value); }
        }

        public string TOTPSettingsStringName
        {
            get { return _pluginHost.CustomConfig.GetString(SettingKeys.TOTPSettingsStringName, Localization.Strings.TOTPSettings); }
            set { _pluginHost.CustomConfig.SetString(SettingKeys.TOTPSettingsStringName, value); }
        }

        public bool FirstInstallShown
        {
            get { return _pluginHost.CustomConfig.GetBool(SettingKeys.FirstInstallShown, false); }
            set { _pluginHost.CustomConfig.SetBool(SettingKeys.FirstInstallShown, value); }
        }

        internal void Reset()
        {
            // Menus
            _pluginHost.CustomConfig.SetString(SettingKeys.EntryContextCopyVisible, null);
            _pluginHost.CustomConfig.SetString(SettingKeys.EntryContextSetupVisible, null);
            _pluginHost.CustomConfig.SetString(SettingKeys.NotifyContextVisible, null);

            // TOTP Column
            _pluginHost.CustomConfig.SetString(SettingKeys.TOTPColumnCopyEnable, null);
            _pluginHost.CustomConfig.SetString(SettingKeys.TOTPColumnTimerVisible, null);

            // Auto-Type
            _pluginHost.CustomConfig.SetString(SettingKeys.AutoTypeEnable, null);

            _pluginHost.CustomConfig.SetString(SettingKeys.AutoTypeFieldName, null);

            // Time Correction
            _pluginHost.CustomConfig.SetString(SettingKeys.TimeCorrectionEnable, null);
            _pluginHost.CustomConfig.SetString(SettingKeys.TimeCorrectionRefreshTime, null);

            // Storage
            _pluginHost.CustomConfig.SetString(SettingKeys.TOTPSeedStringName, null);
            _pluginHost.CustomConfig.SetString(SettingKeys.TOTPSettingsStringName, null);
        }
    }
}
