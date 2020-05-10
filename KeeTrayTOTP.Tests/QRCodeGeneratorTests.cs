using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QRCoder;
using System;
using System.Drawing;
using ZXing;
using ZXing.Common;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class QRCodeGeneratorTests
    {
        [TestMethod]
        public void QRCodeGenerator_ShouldGenerateCodeWithValidKeyUrl()
        {
            var initialCode = "otpauth://totp/Sample%20Entry%20%232:?secret=JBSWY3DPEHPK3PXP&issuer=Sample%20Entry%20%232";
            using (var generator = new QRCoder.QRCodeGenerator())
            {
                var qrCodeData = generator.CreateQrCode(initialCode);

                qrCodeData.Should().NotBeNull();
                qrCodeData.Version.Should().Be(8);
                qrCodeData.ModuleMatrix.Should().HaveCount(57);

                using (var image = GetQRBitmap(qrCodeData))
                {
                    var decodedText = GetTextInQrCode(image);

                    decodedText.Should().Be(initialCode);
                }
            }
        }

        private string GetTextInQrCode(Bitmap bitmap)
        {
            var source = new BitmapLuminanceSource(bitmap);
            var binaryBitmap = new BinaryBitmap(new HybridBinarizer(source));
            var result = new MultiFormatReader().decode(binaryBitmap);
            if (result != null)
            {
                return result.Text;
            }
            else
            {
                throw new InvalidOperationException("Could not decode bitmap as QR");
            }
        }

        private static Bitmap GetQRBitmap(QRCodeData qrCodeData)
        {
            using (var qrCode = new QRCode(qrCodeData))
            {
                int pixelsPerModule = Math.Max(3, 360);

                return qrCode.GetGraphic(pixelsPerModule);
            }
        }
    }
}
