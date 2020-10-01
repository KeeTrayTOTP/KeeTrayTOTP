using System;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using System.Collections.Generic;
using System.Linq;

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

            return pwDocument.CreateRecycleBin();
        }

        public static PwDocument New(this PwDocument pwDocument)
        {
            return pwDocument.NewAs("foobar");
        }

        public static PwDocument CreateRecycleBin(this PwDocument pwDocument)
        {
            var recycleBin = pwDocument.Database.RootGroup.FindCreateGroup("Recycle bin", true);
            pwDocument.Database.RecycleBinUuid = recycleBin.Uuid;

            return pwDocument;
        }

        public static PwDocument WithTotpEnabledEntries(this PwDocument pwDocument, int count)
        {
            return WithTotpEnabledEntries(pwDocument, count, entry => entry);
        }

        public static PwDocument WithTotpEnabledEntries(this PwDocument pwDocument, int count, Func<PwEntry, PwEntry> additionalConfigurations)
        {
            for (int i = 0; i < count; i++)
            {
                var withValidTotpSettings = new PwEntry(true, true).WithValidTotpSettings();
                var pwEntry = additionalConfigurations(withValidTotpSettings);
                pwDocument.Database.RootGroup.AddEntry(pwEntry, true);
            }

            return pwDocument;
        }

        public static PwDocument WithDeletedTotpEnabledEntries(this PwDocument pwDocument, int count)
        {
            var recycleBin = pwDocument.Database.RootGroup.FindGroup(pwDocument.Database.RecycleBinUuid, true);
            for (int i = 0; i < count; i++)
            {
                recycleBin.AddEntry(
                    new PwEntry(true, true).WithValidTotpSettings(),
                    true);
            }

            return pwDocument;
        }

        public static PwDocument WithFaultyTotpEnabledEntries(this PwDocument pwDocument, int count)
        {
            for (int i = 0; i < count; i++)
            {
                pwDocument.Database.RootGroup.AddEntry(
                    new PwEntry(true, true).WithInvalidTotpSettings(),
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

        public static IEnumerable<PwDocument> AsEnumerable(this PwDocument pwDocument)
        {
            yield return pwDocument;
        }

        public static List<PwDocument> AsList(this PwDocument pwDocument)
        {
            return pwDocument.AsEnumerable().ToList();
        }
    }
}
