using System.Collections.Generic;
using KeePass.UI;

namespace KeeTrayTOTP.Helpers
{
    internal static class DocumentHelper
    {
        public static bool IsSingleDatabaseOpenAndUnlocked(this List<PwDocument> documents)
        {
            return documents.Count == 1 && documents[0].Database.IsOpen;
        }

        /// <summary>
        /// The DocumentManager of keepass will always return a list with at least a single "pseudo" document.
        /// This method checks if this single document is the pseudo document.
        /// </summary>
        /// <returns>Returns true if there is no real document open.</returns>
        public static bool IsNotAtLeastOneDocumentOpen(this List<PwDocument> documents)
        {
            return ((documents.Count == 1) && (!documents[0].Database.IsOpen) &&
                    (documents[0].LockedIoc.Path.Length == 0));
        }
    }
}
