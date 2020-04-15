using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class QRCodeGeneratorTests
    {
        [TestMethod]
        public void QRCodeGenerator_ShouldGenerateCodeWithValidKeyUrl()
        {
            var code = "otpauth://totp/Sample%20Entry%20%232:?secret=JBSWY3DPEHPK3PXP&issuer=Sample%20Entry%20%232";
            using (var generator = new QRCoder.QRCodeGenerator())
            {
                var act = generator.CreateQrCode(code);

                act.Should().NotBeNull();
                act.Version.Should().Be(8);
                act.ModuleMatrix.Should().HaveCount(57);
            }
        }
    }
}
