using KeePassLib;
using KeePassLib.Security;

namespace KeeTrayTOTP.Tests.Extensions
{
    internal static class PwEntryExtensions
    {
        internal static PwEntry WithValidTotpSettings(this PwEntry pwEntry)
        {
            pwEntry.Strings.Set(Localization.Strings.TOTPSeed, new ProtectedString(false, "JBSWY3DPEHPK3PXP"));
            pwEntry.Strings.Set(Localization.Strings.TOTPSettings, new ProtectedString(false, "30;6"));
            return pwEntry;
        }

        internal static PwEntry WithInvalidTotpSettings(this PwEntry pwEntry)
        {
            pwEntry.Strings.Set(Localization.Strings.TOTPSeed, new ProtectedString(false, "11111-"));
            pwEntry.Strings.Set(Localization.Strings.TOTPSettings, new ProtectedString(false, "-1;10"));
            return pwEntry;
        }
    }
}
