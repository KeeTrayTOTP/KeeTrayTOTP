using KeePass.App.Configuration;
using System;
using System.Collections.Generic;

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
            internal const string LegacyTrayMenuProviderEnable = "traymenulegacymenuprovider_enable";
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
        }

        private readonly AceCustomConfig _keePassCustomConfig;

        public Settings(AceCustomConfig keepassCustomConfig)
        {
            this._keePassCustomConfig = keepassCustomConfig;
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
            get { return _keePassCustomConfig.GetBool(SettingKeys.EntryContextCopyVisible, true); }
            set { _keePassCustomConfig.SetBool(SettingKeys.EntryContextCopyVisible, value); }
        }

        public bool EntryContextSetupVisible
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.EntryContextSetupVisible, true); }
            set { _keePassCustomConfig.SetBool(SettingKeys.EntryContextSetupVisible, value); }
        }

        public bool NotifyContextVisible
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.NotifyContextVisible, true); }
            set { _keePassCustomConfig.SetBool(SettingKeys.NotifyContextVisible, value); }
        }

        public bool LegacyTrayMenuProviderEnable
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.LegacyTrayMenuProviderEnable, false); }
            set { _keePassCustomConfig.SetBool(SettingKeys.LegacyTrayMenuProviderEnable, value); }
        }

        public bool TrimTrayText
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.TrimTrayText, false); }
            set { _keePassCustomConfig.SetBool(SettingKeys.TrimTrayText, value); }
        }

        public bool TOTPColumnCopyEnable
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.TOTPColumnCopyEnable, true); }
            set { _keePassCustomConfig.SetBool(SettingKeys.TOTPColumnCopyEnable, value); }
        }

        public bool TOTPColumnTimerVisible
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.TOTPColumnTimerVisible, true); }
            set { _keePassCustomConfig.SetBool(SettingKeys.TOTPColumnTimerVisible, value); }
        }

        public bool AutoTypeEnable
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.AutoTypeEnable, true); }
            set { _keePassCustomConfig.SetBool(SettingKeys.AutoTypeEnable, value); }
        }

        public string AutoTypeFieldName
        {
            get { return _keePassCustomConfig.GetString(SettingKeys.AutoTypeFieldName, SettingDefaults.AutoTypeFieldName); }
            set { _keePassCustomConfig.SetString(SettingKeys.AutoTypeFieldName, value); }
        }

        public bool TimeCorrectionEnable
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.TimeCorrectionEnable, false); }
            set { _keePassCustomConfig.SetBool(SettingKeys.TimeCorrectionEnable, value); }
        }

        public ulong TimeCorrectionRefreshTime
        {
            get { return _keePassCustomConfig.GetULong(SettingKeys.TimeCorrectionRefreshTime, SettingDefaults.TimeCorrectionRefreshTime); }
            set { _keePassCustomConfig.SetULong(SettingKeys.TimeCorrectionRefreshTime, value); }
        }

        public IEnumerable<string> TimeCorrectionList
        {
            get { return _keePassCustomConfig.GetString(SettingKeys.TimeCorrectionList, string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries); }
            set { _keePassCustomConfig.SetString(SettingKeys.TimeCorrectionList, string.Join(";", value)); }
        }

        public string TOTPSeedStringName
        {
            get { return _keePassCustomConfig.GetString(SettingKeys.TOTPSeedStringName, Localization.Strings.TOTPSeed); }
            set { _keePassCustomConfig.SetString(SettingKeys.TOTPSeedStringName, value); }
        }

        public string TOTPSettingsStringName
        {
            get { return _keePassCustomConfig.GetString(SettingKeys.TOTPSettingsStringName, Localization.Strings.TOTPSettings); }
            set { _keePassCustomConfig.SetString(SettingKeys.TOTPSettingsStringName, value); }
        }

        public bool FirstInstallShown
        {
            get { return _keePassCustomConfig.GetBool(SettingKeys.FirstInstallShown, false); }
            set { _keePassCustomConfig.SetBool(SettingKeys.FirstInstallShown, value); }
        }

        internal void Reset()
        {
            // Menus
            _keePassCustomConfig.SetString(SettingKeys.EntryContextCopyVisible, null);
            _keePassCustomConfig.SetString(SettingKeys.EntryContextSetupVisible, null);
            _keePassCustomConfig.SetString(SettingKeys.LegacyTrayMenuProviderEnable, null);
            _keePassCustomConfig.SetString(SettingKeys.NotifyContextVisible, null);

            // TOTP Column
            _keePassCustomConfig.SetString(SettingKeys.TOTPColumnCopyEnable, null);
            _keePassCustomConfig.SetString(SettingKeys.TOTPColumnTimerVisible, null);

            // Auto-Type
            _keePassCustomConfig.SetString(SettingKeys.AutoTypeEnable, null);

            _keePassCustomConfig.SetString(SettingKeys.AutoTypeFieldName, null);

            // Time Correction
            _keePassCustomConfig.SetString(SettingKeys.TimeCorrectionEnable, null);
            _keePassCustomConfig.SetString(SettingKeys.TimeCorrectionRefreshTime, null);

            // Storage
            _keePassCustomConfig.SetString(SettingKeys.TOTPSeedStringName, null);
            _keePassCustomConfig.SetString(SettingKeys.TOTPSettingsStringName, null);
        }
    }
}
