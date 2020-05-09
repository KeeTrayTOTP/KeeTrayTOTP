using System.Collections.Generic;

namespace KeeTrayTOTP
{
    public interface ISettings
    {
        bool AutoTypeEnable { get; set; }
        string AutoTypeFieldName { get; set; }
        bool EntryContextCopyVisible { get; set; }
        bool EntryContextSetupVisible { get; set; }
        int EntryListRefreshRate { get; }
        bool FirstInstallShown { get; set; }
        bool LegacyTrayMenuProviderEnable { get; set; }
        bool NotifyContextVisible { get; set; }
        bool TimeCorrectionEnable { get; set; }
        IEnumerable<string> TimeCorrectionList { get; set; }
        ulong TimeCorrectionRefreshTime { get; set; }
        bool TOTPColumnCopyEnable { get; set; }
        bool TOTPColumnTimerVisible { get; set; }
        string TOTPSeedStringName { get; set; }
        string TOTPSettingsStringName { get; set; }
        int TrimTextLength { get; }
        bool TrimTrayText { get; set; }
    }
}