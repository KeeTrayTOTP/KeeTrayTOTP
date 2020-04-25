using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePass.UI;

namespace KeeTrayTOTP.Tests.Extensions
{
    public static class PwDocumentExtensions
    {
        public static PwDocument Lock(this PwDocument pwDocument)
        {
            pwDocument.LockedIoc = pwDocument.Database.IOConnectionInfo.CloneDeep();
            pwDocument.Database.Close();
            return pwDocument;
        }
    }
}
