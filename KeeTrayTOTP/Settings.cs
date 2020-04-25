using KeePass.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeeTrayTOTP
{
    public class Settings
    {
        private readonly IPluginHost _pluginHost;

        public Settings(IPluginHost pluginHost)
        {
            this._pluginHost = pluginHost;
        }

        public bool EntryContextCopyVisible
        {
            get
            {
                return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_EntryContextCopy_Visible, true);
            }
            set
            {
                _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_EntryContextCopy_Visible, value);
            }
        }

        public bool EntryContextSetupVisible
        {
            get
            {
                return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_EntryContextSetup_Visible, true);
            }
            set
            {
                _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_EntryContextSetup_Visible, value);
            }
        }

        public bool NotifyContextVisible
        {
            get
            {
                return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_NotifyContext_Visible, true);
            }
            set
            {
                _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_NotifyContext_Visible, value);
            }
        }

        public bool TrimTrayText
        {
            get
            {
                return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);
            }
            set
            {
                _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, value);
            }
        }

        public bool TOTPColumnCopyEnable
        {
            get
            {
                return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnCopy_Enable, true);
            }
            set
            {
                _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnCopy_Enable, value);
            }
        }

        public bool TOTPColumnTimerVisible
        {
            get
            {
                return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnTimer_Visible, true);
            }
            set
            {
                _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnTimer_Visible, value);
            }
        }

        public bool AutoTypeEnable
        {
            get
            {
                return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_AutoType_Enable, true);
            }
            set
            {
                _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_AutoType_Enable, value);
            }
        }

        public string AutoTypeFieldName
        {
            get
            {
                return _pluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_AutoType_FieldName, KeeTrayTOTPExt.setdef_string_AutoType_FieldName);
            }
            set
            {
                _pluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_AutoType_FieldName, value);
            }
        }

        public bool TimeCorrectionEnable
        {
            get
            {
                return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TimeCorrection_Enable, false);
            }
            set
            {
                _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_TimeCorrection_Enable, value);
            }
        }

        public ulong TimeCorrectionRefreshTime
        {
            get
            {
                return _pluginHost.CustomConfig.GetULong(KeeTrayTOTPExt.setname_ulong_TimeCorrection_RefreshTime, KeeTrayTOTPExt.setdef_TimeCorrection_RefreshTime);
            }
            set
            {
                _pluginHost.CustomConfig.SetULong(KeeTrayTOTPExt.setname_ulong_TimeCorrection_RefreshTime, value);
            }
        }

        public IEnumerable<string> TimeCorrectionList
        {
            get
            {
                return _pluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TimeCorrection_List, string.Empty).Split(';');
            }
            set
            {
                _pluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_TimeCorrection_List, string.Join(";", value));
            }
        }

        public string TOTPSeedStringName
        {
            get
            {
                return _pluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed);
            }
            set
            {
                _pluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, value);
            }
        }

        public string TOTPSettingsStringName
        {
            get
            {
                return _pluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings);
            }
            set
            {
                _pluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, value);
            }
        }

        public bool FirstInstallShown
        {
            get {  return _pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_FirstInstall_Shown, false); }
            set {  _pluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_FirstInstall_Shown, value); }
        }
    }
}
