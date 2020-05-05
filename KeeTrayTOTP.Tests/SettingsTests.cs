using FluentAssertions;
using KeePass.App.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class SettingsTests
    {
        private readonly Settings sut = new Settings(new AceCustomConfig());

        [TestMethod]
        public void AutoTypeEnable_GetAndSetChangeCorrectField()
        {
            sut.AutoTypeEnable.Should().BeTrue("because that is the default value");
            sut.AutoTypeEnable = !sut.AutoTypeEnable;
            sut.AutoTypeEnable.Should().BeFalse("because the setting was changed to a non-default value");
            sut.Reset();
            sut.AutoTypeEnable.Should().BeTrue("because settings are reset");
        }

        [TestMethod]
        public void AutoTypeFieldName_GetAndSetChangeCorrectField()
        {
            sut.AutoTypeFieldName.Should().Be("TOTP", "because that is the default value");
            sut.AutoTypeFieldName = "xxxx";
            sut.AutoTypeFieldName.Should().Be("xxxx", "because the setting was changed to a non-default value");
            sut.Reset();
            sut.AutoTypeFieldName.Should().Be("TOTP", "because settings are reset");
        }

        [TestMethod]
        public void EntryContextCopyVisible_GetAndSetChangeCorrectField()
        {
            sut.EntryContextCopyVisible.Should().BeTrue("because that is the default value");
            sut.EntryContextCopyVisible = !sut.EntryContextCopyVisible;
            sut.EntryContextCopyVisible.Should().BeFalse("because the setting was changed to a non-default value");
            sut.Reset();
            sut.EntryContextCopyVisible.Should().BeTrue("because settings are reset");
        }

        [TestMethod]
        public void EntryContextSetupVisible_GetAndSetChangeCorrectField()
        {
            sut.EntryContextSetupVisible.Should().BeTrue("because that is the default value");
            sut.EntryContextSetupVisible = !sut.EntryContextSetupVisible;
            sut.EntryContextSetupVisible.Should().BeFalse("because the setting was changed to a non-default value");
            sut.Reset();
            sut.EntryContextSetupVisible.Should().BeTrue("because settings are reset");
        }

        [TestMethod]
        public void EntryListRefreshRate_GetReturnsDefault()
        {
            sut.EntryListRefreshRate.Should().Be(300, "because that is the default value");
        }

        [TestMethod]
        public void FirstInstallShown_GetAndSetChangeCorrectField()
        {
            sut.FirstInstallShown.Should().BeFalse("because that is the default value");
            sut.FirstInstallShown = !sut.FirstInstallShown;
            sut.FirstInstallShown.Should().BeTrue("because the setting was changed to a non-default value");
        }

        [TestMethod]
        public void LegacyTrayMenuProviderEnable_GetAndSetChangeCorrectField()
        {
            sut.LegacyTrayMenuProviderEnable.Should().BeFalse("because that is the default value");
            sut.LegacyTrayMenuProviderEnable = !sut.FirstInstallShown;
            sut.LegacyTrayMenuProviderEnable.Should().BeTrue("because the setting was changed to a non-default value");
            sut.Reset();
            sut.LegacyTrayMenuProviderEnable.Should().BeFalse("because settings are reset");
        }

        [TestMethod]
        public void NotifyContextVisible_GetAndSetChangeCorrectField()
        {
            sut.NotifyContextVisible.Should().BeTrue("because that is the default value");
            sut.NotifyContextVisible = !sut.NotifyContextVisible;
            sut.NotifyContextVisible.Should().BeFalse("because the setting was changed to a non-default value");
            sut.Reset();
            sut.NotifyContextVisible.Should().BeTrue("because settings are reset");
        }

        [TestMethod]
        public void TimeCorrectionEnable_GetAndSetChangeCorrectField()
        {
            sut.TimeCorrectionEnable.Should().BeFalse("because that is the default value");
            sut.TimeCorrectionEnable = !sut.TimeCorrectionEnable;
            sut.TimeCorrectionEnable.Should().BeTrue("because the setting was changed to a non-default value");
            sut.Reset();
            sut.TimeCorrectionEnable.Should().BeFalse("because settings are reset");
        }

        [TestMethod]
        public void TimeCorrectionList_GetAndSetChangeCorrectField()
        {
            sut.TimeCorrectionList.Should().HaveCount(0, "because that is the default value");
            sut.TimeCorrectionList = new[] { "Url1", "Url2", "Url3" };
            sut.TimeCorrectionList.Should().BeEquivalentTo(new[] { "Url1", "Url2", "Url3" });
        }

        [TestMethod]
        public void TimeCorrectionRefreshTime_GetAndSetChangeCorrectField()
        {
            sut.TimeCorrectionRefreshTime.Should().Be(60, "because that is the default value");
            sut.TimeCorrectionRefreshTime += 100;
            sut.TimeCorrectionRefreshTime.Should().Be(160, "because the setting was changed to a non-default value");
            sut.Reset();
            sut.TimeCorrectionRefreshTime.Should().Be(60, "because settings are reset");
        }

        [TestMethod]
        public void TOTPColumnCopyEnable_GetAndSetChangeCorrectField()
        {
            sut.TOTPColumnCopyEnable.Should().BeTrue("because that is the default value");
            sut.TOTPColumnCopyEnable = !sut.TOTPColumnCopyEnable;
            sut.TOTPColumnCopyEnable.Should().BeFalse("because the setting was changed to a non-default value");
            sut.Reset();
            sut.TOTPColumnCopyEnable.Should().BeTrue("because settings are reset");
        }

        [TestMethod]
        public void TOTPColumnTimerVisible_GetAndSetChangeCorrectField()
        {
            sut.TOTPColumnTimerVisible.Should().BeTrue("because that is the default value");
            sut.TOTPColumnTimerVisible = !sut.TOTPColumnTimerVisible;
            sut.TOTPColumnTimerVisible.Should().BeFalse("because the setting was changed to a non-default value");
            sut.Reset();
            sut.TOTPColumnTimerVisible.Should().BeTrue("because settings are reset");
        }

        [TestMethod]
        public void TOTPSeedStringName_GetAndSetChangeCorrectField()
        {
            sut.TOTPSeedStringName.Should().Be("TOTP Seed", "because that is the default value");
            sut.TOTPSeedStringName = "SeedSeedSeed";
            sut.TOTPSeedStringName.Should().Be("SeedSeedSeed", "because the setting was changed to a non-default value");
            sut.Reset();
            sut.TOTPSeedStringName.Should().Be("TOTP Seed", "because settings are reset");
        }

        [TestMethod]
        public void TOTPSettingsStringName_GetAndSetChangeCorrectField()
        {
            sut.TOTPSettingsStringName.Should().Be("TOTP Settings", "because that is the default value");
            sut.TOTPSettingsStringName = "SettingsSettingsSettings";
            sut.TOTPSettingsStringName.Should().Be("SettingsSettingsSettings", "because the setting was changed to a non-default value");
            sut.Reset();
            sut.TOTPSettingsStringName.Should().Be("TOTP Settings", "because settings are reset");
        }

        [TestMethod]
        public void PreferKeyUri_GetAndSetChangeCorrectField()
        {
            sut.PreferKeyUri.Should().BeTrue("because that is the default value");
            sut.PreferKeyUri = false;
            sut.PreferKeyUri.Should().BeFalse("because the setting was changed to a non-default value");
            sut.Reset();
            sut.PreferKeyUri.Should().BeTrue("because settings are reset");
        }

        [TestMethod]
        public void TOTPKeyUriStringName_GetAndSetChangeCorrectField()
        {
            sut.TOTPKeyUriStringName.Should().Be("KeyUri", "because that is the default value");
            sut.TOTPKeyUriStringName = "KeyUriKeyUriKeyUri";
            sut.TOTPKeyUriStringName.Should().Be("KeyUriKeyUriKeyUri", "because the setting was changed to a non-default value");
            sut.Reset();
            sut.TOTPKeyUriStringName.Should().Be("KeyUri", "because settings are reset");
        }

        [TestMethod]
        public void TrimTextLength_GetReturnsDefault()
        {
            sut.TrimTextLength.Should().Be(25, "because that is the default value");
        }

        [TestMethod]
        public void TrimTrayText_GetAndSetChangeCorrectField()
        {
            sut.TrimTrayText.Should().BeFalse("because that is the default value");
            sut.TrimTrayText = !sut.TrimTrayText;
            sut.TrimTrayText.Should().BeTrue("because the setting was changed to a non-default value");
        }
    }
}
