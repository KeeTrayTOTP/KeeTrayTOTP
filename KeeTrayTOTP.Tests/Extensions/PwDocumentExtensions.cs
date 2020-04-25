using KeePass.UI;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;

namespace KeeTrayTOTP.Tests.Extensions
{
    public static class PwDocumentExtensions
    {
        public static PwDocument Locked(this PwDocument pwDocument)
        {
            pwDocument.LockedIoc = pwDocument.Database.IOConnectionInfo.CloneDeep();
            pwDocument.Database.Close();
            return pwDocument;
        }

        public static PwDocument NewAs(this PwDocument pwDocument, string filename)
        {
            pwDocument.Database.New(IOConnectionInfo.FromPath(filename), new CompositeKey());
            return pwDocument;
        }

        public static PwDocument New(this PwDocument pwDocument)
        {
            return pwDocument.NewAs("foobar");
        }

        public static PwDocument WithTotpEnabledEntries(this PwDocument pwDocument, int count)
        {
            for (int i = 0; i < count; i++)
            {
                pwDocument.Database.RootGroup.AddEntry(
                    new PwEntry(true, true).WithValidTotpSettings(),
                    true);
            }
            
            return pwDocument;
        }

        public static PwDocument WithNonTotpEntries(this PwDocument pwDocument, int count)
        {
            for (int i = 0; i < count; i++)
            {
                pwDocument.Database.RootGroup.AddEntry(
                    new PwEntry(true, true),
                    true);
            }

            return pwDocument;
        }
    }
}
