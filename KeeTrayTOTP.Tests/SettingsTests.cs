using FluentAssertions;
using KeePass.App.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class SettingsTests
    {
        private readonly Settings sut;

        public SettingsTests()
        {
            var customConfig = new AceCustomConfig();
            sut = new Settings(customConfig);
        }

        [TestMethod]
        public void AutoTypeEnable_GetAndSetChangeCorrectField()
        {
            sut.AutoTypeEnable.Should().BeTrue();
            sut.AutoTypeEnable = !sut.AutoTypeEnable;
            sut.AutoTypeEnable.Should().BeFalse();
        }

        [TestMethod]
        public void AutoTypeFieldName_GetAndSetChangeCorrectField()
        {
            sut.AutoTypeFieldName.Should().Be("TOTP");
            sut.AutoTypeFieldName = "xxxx";
            sut.AutoTypeFieldName.Should().Be("xxxx");
        }

        [TestMethod]
        public void EntryContextCopyVisible_GetAndSetChangeCorrectField()
        {
            sut.EntryContextCopyVisible.Should().BeTrue();
            sut.EntryContextCopyVisible = !sut.EntryContextCopyVisible;
            sut.EntryContextCopyVisible.Should().BeFalse();
        }

        [TestMethod]
        public void EntryContextSetupVisible_GetAndSetChangeCorrectField()
        {
            sut.EntryContextSetupVisible.Should().BeTrue();
            sut.EntryContextSetupVisible = !sut.EntryContextSetupVisible;
            sut.EntryContextSetupVisible.Should().BeFalse();
        }

        [TestMethod]
        public void EntryListRefreshRate_GetReturnsDefault()
        {
            sut.EntryListRefreshRate.Should().Be(300);
        }

        [TestMethod]
        public void FirstInstallShown_GetAndSetChangeCorrectField()
        {
            sut.FirstInstallShown.Should().BeFalse();
            sut.FirstInstallShown = !sut.FirstInstallShown;
            sut.FirstInstallShown.Should().BeTrue();
        }

        [TestMethod]
        public void NotifyContextVisible_GetAndSetChangeCorrectField()
        {
            sut.NotifyContextVisible.Should().BeTrue();
            sut.NotifyContextVisible = !sut.NotifyContextVisible;
            sut.NotifyContextVisible.Should().BeFalse();
        }

        [TestMethod]
        public void TimeCorrectionEnable_GetAndSetChangeCorrectField()
        {
            sut.TimeCorrectionEnable.Should().BeFalse();
            sut.TimeCorrectionEnable = !sut.TimeCorrectionEnable;
            sut.TimeCorrectionEnable.Should().BeTrue();
        }

        [TestMethod]
        public void TimeCorrectionList_GetAndSetChangeCorrectField()
        {
            sut.TimeCorrectionList.Should().HaveCount(0);
            sut.TimeCorrectionList = new [] { "Url1", "Url2", "Url3"};
            sut.TimeCorrectionList.Should().BeEquivalentTo(new[] { "Url1", "Url2", "Url3" });
        }

        [TestMethod]
        public void TimeCorrectionRefreshTime_GetAndSetChangeCorrectField()
        {
            sut.TimeCorrectionRefreshTime.Should().Be(60);
            sut.TimeCorrectionRefreshTime += 100;
            sut.TimeCorrectionRefreshTime.Should().Be(160);
        }

        [TestMethod]
        public void TOTPColumnCopyEnable_GetAndSetChangeCorrectField()
        {
            sut.TOTPColumnCopyEnable.Should().BeTrue();
            sut.TOTPColumnCopyEnable = !sut.TOTPColumnCopyEnable;
            sut.TOTPColumnCopyEnable.Should().BeFalse();
        }

        [TestMethod]
        public void TOTPColumnTimerVisible_GetAndSetChangeCorrectField()
        {
            sut.TOTPColumnTimerVisible.Should().BeTrue();
            sut.TOTPColumnTimerVisible = !sut.TOTPColumnTimerVisible;
            sut.TOTPColumnTimerVisible.Should().BeFalse();
        }

        [TestMethod]
        public void TOTPSeedStringName_GetAndSetChangeCorrectField()
        {
            sut.TOTPSeedStringName.Should().Be("TOTP Seed");
            sut.TOTPSeedStringName = "SeedSeedSeed";
            sut.TOTPSeedStringName.Should().Be("SeedSeedSeed");
        }

        [TestMethod]
        public void TOTPSettingsStringName_GetAndSetChangeCorrectField()
        {
            sut.TOTPSettingsStringName.Should().Be("TOTP Settings");
            sut.TOTPSettingsStringName = "SettingsSettingsSettings";
            sut.TOTPSettingsStringName.Should().Be("SettingsSettingsSettings");
        }

        [TestMethod]
        public void TrimTextLength_GetReturnsDefault()
        {
            sut.TrimTextLength.Should().Be(25);
        }

        [TestMethod]
        public void TrimTrayText_GetAndSetChangeCorrectField()
        {
            sut.TrimTrayText.Should().BeFalse();
            sut.TrimTrayText = !sut.TrimTrayText;
            sut.TrimTrayText.Should().BeTrue();
        }
    }
}
