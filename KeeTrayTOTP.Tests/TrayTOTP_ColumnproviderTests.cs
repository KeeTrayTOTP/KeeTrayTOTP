using FluentAssertions;
using KeePass.App.Configuration;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class TrayTOTP_ColumnproviderTests : IDisposable
    {
        private readonly KeeTrayTOTPExt _plugin;
        private readonly IPluginHost _pluginHost;

        const string InvalidSeed = "C5CYMIHWQUUZMKUGZHGEOSJSQDE4L===!";
        const string ValidSeed = "JBSWY3DPEHPK3PXP";
        const string ValidSettings = "30;6";

        public TrayTOTP_ColumnproviderTests()
        {
            (_plugin, _pluginHost) = CreateInitializedPlugin();
        }

        [DataRow(ValidSeed, ValidSettings, "TOTP Enabled")]
        [DataRow(ValidSeed, ";6", "Error, bad settings!")]
        [DataRow(ValidSeed, "30", "Error, bad settings!")]
        [DataRow(ValidSeed, null, "Error, storage!")]
        [DataRow(InvalidSeed, ValidSettings, "Error, bad seed!")]
        [DataRow(null, ValidSettings, "Error, storage!")]
        [DataRow(null, null, "")]
        [DataTestMethod]
        public void GetCellDataStatus_ShouldReturnExpectedValues(string seed, string settings, string expected)
        {
            var column = new TrayTOTP_ColumnProvider(_plugin);
            var pwEntry = new KeePassLib.PwEntry(true, true);
            if (seed != null)
            {
                pwEntry.Strings.Set(_plugin.Settings.TOTPSeedStringName, new ProtectedString(false, seed));
            }
            if (settings != null)
            {
                pwEntry.Strings.Set(_plugin.Settings.TOTPSettingsStringName, new ProtectedString(false, settings));
            }

            var actual = column.GetCellData("TOTP Status", pwEntry);

            actual.Should().Be(expected);
        }

        [DataRow(ValidSeed, ";6", "Error, bad settings!")]
        [DataRow(ValidSeed, "30", "Error, bad settings!")]
        [DataRow(ValidSeed, null, "Error, storage!")]
        [DataRow(InvalidSeed, ValidSettings, "Error, bad seed!")]
        [DataRow(null, ValidSettings, "Error, storage!")]
        [DataRow(null, null, "")]
        [DataTestMethod]
        public void GetCellDataCode_ShouldReturnExpectedValues(string seed, string settings, string expected)
        {
            var column = new TrayTOTP_ColumnProvider(_plugin);
            var pwEntry = new KeePassLib.PwEntry(true, true);
            if (seed != null)
            {
                pwEntry.Strings.Set(_plugin.Settings.TOTPSeedStringName, new ProtectedString(false, seed));
            }
            if (settings != null)
            {
                pwEntry.Strings.Set(_plugin.Settings.TOTPSettingsStringName, new ProtectedString(false, settings));

            }

            var actual = column.GetCellData("TOTP", pwEntry);

            actual.Should().Be(expected);
        }

        [DataRow(true, @"^\d{6} \(\d{1,2}\)$", DisplayName = "Column timer visible should show a code and validity")]
        [DataRow(false, @"^\d{6}$", DisplayName = "Column timer invisible should only show a code")]
        [DataTestMethod]
        public void GetCellDataCode_WithValidSeedAndSettings_ShouldReturnA6DigitCodeWithDuration(bool showTimer, string regex)
        {
            _plugin.Settings.TOTPColumnTimerVisible = showTimer;

            var column = new TrayTOTP_ColumnProvider(_plugin);
            var pwEntry = new KeePassLib.PwEntry(true, true);
            pwEntry.Strings.Set(_plugin.Settings.TOTPSeedStringName, new ProtectedString(false, ValidSeed));
            pwEntry.Strings.Set(_plugin.Settings.TOTPSettingsStringName, new ProtectedString(false, ValidSettings));

            var actual = column.GetCellData("TOTP", pwEntry);

            actual.Should().MatchRegex(regex);
        }

        [TestMethod]
        public void GetCellData_WithAnInvalidColumn_ShouldReturnEmptyString()
        {
            var column = new TrayTOTP_ColumnProvider(_plugin);
            var pwEntry = new KeePassLib.PwEntry(true, true);
            
            pwEntry.Strings.Set(_plugin.Settings.TOTPSeedStringName, new ProtectedString(false, ValidSeed));
            pwEntry.Strings.Set(_plugin.Settings.TOTPSettingsStringName, new ProtectedString(false, ValidSettings));

            var actual = column.GetCellData("InvalidColumnName", pwEntry);

            actual.Should().BeEmpty();
        }

        private static (KeeTrayTOTPExt, IPluginHost) CreateInitializedPlugin()
        {
            var plugin = new KeeTrayTOTPExt();
            var pluginHost = new Mock<IPluginHost>(MockBehavior.Strict);

            var keepassForm = new MainForm();
            pluginHost.SetupGet(c => c.MainWindow).Returns(keepassForm);

            var customConfig = new AceCustomConfig();
            pluginHost.SetupGet(c => c.CustomConfig).Returns(customConfig);

            var columnProviderPool = new ColumnProviderPool();
            pluginHost.SetupGet(c => c.ColumnProviderPool).Returns(columnProviderPool);

            plugin.Initialize(pluginHost.Object);

            return (plugin, pluginHost.Object);
        }

        public void Dispose()
        {
            _plugin.Terminate();
        }
    }
}
