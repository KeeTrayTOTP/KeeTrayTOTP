using FluentAssertions;
using KeePassLib;
using KeePassLib.Security;
using KeeTrayTOTP.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class TOTPEntryValidatorTests
    {
        private readonly Mock<ISettings> _settingsMock;
        private readonly TOTPEntryValidator _sut;
        private readonly PwEntry _entry;

        public TOTPEntryValidatorTests()
        {
            this._settingsMock = new Mock<ISettings>(MockBehavior.Strict);
            this._settingsMock.Setup(c => c.TOTPSettingsStringName).Returns("TOTP Settings");
            this._settingsMock.Setup(c => c.TOTPSeedStringName).Returns("TOTP Seed");
            this._sut = new TOTPEntryValidator(_settingsMock.Object);
            this._entry = new PwEntry(true, false);
        }

        [TestMethod]
        public void CanGenerateOtp_EntryHasNoSettingsOrSeed_ReturnsFalse()
        {
            _entry.Strings.Set(_settingsMock.Object.TOTPSeedStringName, new ProtectedString(false, "ABCDEFG"));
            var act = _sut.CanGenerateTOTP(_entry);

            act.Should().BeTrue();
        }

        [TestMethod]
        public void CanGenerateOtp_EntryOnlyHasSeed_ReturnsTrue()
        {
            var act = _sut.CanGenerateTOTP(_entry);

            act.Should().BeFalse();
        }

        [TestMethod]
        public void SettingsGet_EntryHasNoSettings_ReturnsDefaultSettings()
        {
            var act = _sut.SettingsGet(_entry);

            act.Should().BeEquivalentTo("30", "6");
        }

        [TestMethod]
        public void SettingsGet_EntryHasSettings_ShouldReturnOwnSettings()
        {
            _entry.Strings.Set(_settingsMock.Object.TOTPSettingsStringName, new ProtectedString(false, "60;7;https://pool.ntp.org"));

            var act = _sut.SettingsGet(_entry);

            act.Should().BeEquivalentTo("60", "7", "https://pool.ntp.org");
        }
    }
}
