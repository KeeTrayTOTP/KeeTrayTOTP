using System.Drawing;
using KeePass.UI;

namespace KeeTrayTOTP.Helpers
{
    internal static class ImageExtensions
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
