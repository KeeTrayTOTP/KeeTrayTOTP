using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class KeyUriTests
    {
        private const string MinimalKeyUri = "otpauth://totp/ACME%20Co:john.doe@email.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co";

        [TestMethod]
        public void Ctor_ShouldInitializeValidTotpKeyUri()
        {
            var uri = new Uri("otpauth://totp/ACME%20Co:john.doe@email.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30");

            var keyUri = new KeyUri(uri);
            keyUri.Type.Should().Be("totp");
            keyUri.Secret.Should().Be("HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ");
            keyUri.Issuer.Should().Be("ACME Co");
            keyUri.Algorithm.Should().Be("SHA1");
            keyUri.Digits.Should().Be(6);
            keyUri.Period.Should().Be(30);
        }

        [TestMethod]
        public void Ctor_ShouldValidateUri()
        {
            Action act = () => new KeyUri(null);

            act.Should().ThrowExactly<ArgumentNullException>().WithMessage("Uri should not be null.*");
        }

        [TestMethod]
        public void Ctor_ShouldValidateScheme()
        {
            var uri = new Uri("xxxotpauth://totp/ACME%20Co:john.doe@email.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30");

            Action act = () => new KeyUri(uri);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("Uri scheme must be otpauth.*");
        }

        [TestMethod]
        public void Ctor_ShouldValidateType()
        {
            var uri = new Uri("otpauth://hotp/ACME%20Co:john.doe@email.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30");

            Action act = () => new KeyUri(uri);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("Only totp is supported.*");
        }

        [TestMethod]
        public void Ctor_ShouldValidateSecret()
        {
            var uri = new Uri("otpauth://totp/ACME%20Co:john.doe@email.com?secret=!!HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ");

            Action act = () => new KeyUri(uri);

            act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("Secret is not valid base32.*");
        }

        [TestMethod]
        public void MinimalUrl_ShouldDefaultAlgorithmToSha1()
        {
            var uri = new Uri(MinimalKeyUri);

            var keyUri = new KeyUri(uri);
            keyUri.Algorithm.Should().Be("SHA1");
        }

        [TestMethod]
        public void GetUri_ShouldContainNonDefaultParameters()
        {
            var uri = new Uri(MinimalKeyUri);

            var keyUri = new KeyUri(uri);
            keyUri.Algorithm = "SHA256";
            keyUri.Digits = 8;
            keyUri.Period = 60;

            var absoluteUri = keyUri.GetUri().AbsoluteUri;
            absoluteUri.Should().Contain("period=60");
            absoluteUri.Should().Contain("digits=8");
            absoluteUri.Should().Contain("algorithm=SHA256");
        }

        [TestMethod]
        public void GetUri_ShouldContainDefaultParameters()
        {
            var uri = new Uri(MinimalKeyUri);

            var keyUri = new KeyUri(uri);
            keyUri.Algorithm = "SHA1";
            keyUri.Digits = 6;
            keyUri.Period = 30;

            var absoluteUri = keyUri.GetUri().AbsoluteUri;
            absoluteUri.Should().NotContain("period=");
            absoluteUri.Should().NotContain("digits=");
            absoluteUri.Should().NotContain("algorithm=");
        }

        [TestMethod]
        public void MinimalUrl_ShouldDefaultPeriodTo30()
        {
            var uri = new Uri(MinimalKeyUri);

            var keyUri = new KeyUri(uri);
            keyUri.Period.Should().Be(30);
        }

        [TestMethod]
        public void MinimalUrl_ShouldDefaultDigitsTo6()
        {
            var uri = new Uri(MinimalKeyUri);

            var keyUri = new KeyUri(uri);
            keyUri.Digits.Should().Be(6);
        }

        [TestMethod]
        public void TestFullKeyUri()
        {
            var model = new KeyUri(new Uri("otpauth://totp/ACME%20Co:john.doe@email.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30"));
            var modelUri = model.GetUri();
            var generatedUri = new KeyUri(modelUri);

            model.Issuer.Should().Be(generatedUri.Issuer);
            model.Label.Should().Be(generatedUri.Label);
            model.Secret.Should().Be(generatedUri.Secret);
            model.Digits.Should().Be(generatedUri.Digits);
            model.Algorithm.Should().Be(generatedUri.Algorithm);
            model.Period.Should().Be(generatedUri.Period);
        }

        // Testcases kindly borrowed from: https://github.com/Aftnet/OTPManager/blob/7ba3c4a34bce6ddc83c040de84e36faed4cde60e/OTPManager.Shared.Test/Components/OTPUriConverterTest.cs
        [DataTestMethod]
        [DataRow("otpauth://totp/Test:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=Test", "Alice Loller@test.com", "Test")]
        [DataRow("otpauth://totp/Test:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "Alice Loller@test.com", "Test")]
        [DataRow("otpauth://totp/Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=Test", "Alice Loller@test.com", "Test")]
        [DataRow("otpauth://totp/Omg:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=Test", "Alice Loller@test.com", "Test")]
        [DataRow("otpauth://totp/Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "Alice Loller@test.com", "")]
        [DataRow("otpauth://totp/Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=", "Alice Loller@test.com", "")]
        [DataRow("otpauth://totp/:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "Alice Loller@test.com", "")]
        public void LabelAndIssuerAreCorrectlyParsed(string uriString, string expectedLabel, string expectedIssuer)
        {
            var model = new KeyUri(new Uri(uriString));

            model.Label.Should().Be(expectedLabel);
            model.Issuer.Should().Be(expectedIssuer);
        }

        // Testcases kindly borrowed from: https://github.com/Aftnet/OTPManager/blob/7ba3c4a34bce6ddc83c040de84e36faed4cde60e/OTPManager.Shared.Test/Components/OTPUriConverterTest.cs
        [DataTestMethod]
        [DataRow("tpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "Uri scheme must be otpauth.")]
        [DataRow("otpauth://hotp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "Only totp is supported.")]
        [DataRow("otpauth://totp/?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "No label")]
        [DataRow("otpauth://totp/SomeLabel?secret=&algorithm=SHA256&digits=6", "No secret provided.")]
        [DataRow("otpauth://totp/SomeLabel?algorithm=SHA256&digits=6", "No secret provided.")]
        [DataRow("otpauth://totp/SomeLabel?secret=ABABABA%3D%3D%3DABABAB&algorithm=SHA256&digits=6", "Secret is not valid base32.")]
        [DataRow("otpauth://totp/SomeLabel?secret=ABABABAB@ABABABAB&algorithm=SHA256&digits=6", "Secret is not valid base32.")]
        [DataRow("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=&digits=6", "Not a valid algorithm")]
        [DataRow("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA12&digits=6", "Not a valid algorithm")]
        [DataRow("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&digits=", "Digits not a number")]
        [DataRow("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&digits=d", "Digits not a number")]
        [DataRow("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&period=", "Period not a number")]
        [DataRow("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&period=d", "Period not a number")]
        public void InvalidUriThrowsArgumentOutOfRangeException(string uriString, string msg)
        {
            Action act = () => new KeyUri(new Uri(uriString));

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage(msg + "*");
        }

        [DataTestMethod]
        [DataRow("30;6;https://www.nist.gov", "ABABABABABABABAB", "otpauth://totp/SomeIssuer:SomeLabel?secret=ABABABABABABABAB&issuer=SomeIssuer&timecorrectionurl=https%3A%2F%2Fwww.nist.gov%2F")]
        [DataRow("30;6", "ABABABABABABABAB", "otpauth://totp/SomeIssuer:SomeLabel?secret=ABABABABABABABAB&issuer=SomeIssuer")]
        [DataRow("30;5", "ABABABABABABABAB", "otpauth://totp/SomeIssuer:SomeLabel?digits=5&secret=ABABABABABABABAB&issuer=SomeIssuer")]
        [DataRow("30;7", "ABABABABABABABAB", "otpauth://totp/SomeIssuer:SomeLabel?digits=7&secret=ABABABABABABABAB&issuer=SomeIssuer")]
        [DataRow("30;S", "ABABABABABABABAB", "otpauth://totp/SomeIssuer:SomeLabel?digits=5&format=Steam&secret=ABABABABABABABAB&issuer=SomeIssuer")]
        [DataRow("60;S;https://store.steampowered.com/", "ABABABABABABABAB", "otpauth://totp/SomeIssuer:SomeLabel?period=60&digits=5&format=Steam&secret=ABABABABABABABAB&issuer=SomeIssuer&timecorrectionurl=https%3A%2F%2Fstore.steampowered.com%2F")]
        public void KeyUri_SerializeDeserialize_ShouldPreserveInformation(string inputSettings, string secret, string expectedUri)
        {
            var settings = inputSettings.Split(';');
            var keyUri = KeyUri.CreateFromLegacySettings(settings, secret);

            // Validate
            keyUri.Secret.Should().Be(secret);
            keyUri.Algorithm.Should().Be("SHA1");

            keyUri.GetUri().AbsoluteUri.Should().Be(expectedUri);

            // Convert the KeyUri instance made from settings and seed, parse the generated uri in a new instance and compare the two.
            var keyUriFromLegacyKeyUri = new KeyUri(keyUri.GetUri());
            keyUriFromLegacyKeyUri.Should().BeEquivalentTo(keyUri);
        }
    }
}
