using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePass.UI;

namespace KeeTrayTOTP.Helpers
{
    public static class ImageHelper
    {
        public static Image CreateImageFromColor(Color color)
        {
            if (color != Color.Empty)
            {
                return UIUtil.CreateColorBitmap24(16, 16, color);
            }

            return null;
        }
    }
}
