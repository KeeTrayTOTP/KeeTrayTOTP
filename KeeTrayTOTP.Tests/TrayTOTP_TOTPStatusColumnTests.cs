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
    public class TrayTOTP_TOTPStatusColumnTests : IDisposable
    {
        private readonly KeeTrayTOTPExt _plugin;
        private readonly IPluginHost _pluginHost;

        public TrayTOTP_TOTPStatusColumnTests()
        {
            (_plugin, _pluginHost) = CreateInitializedPlugin();
        }

        [DataRow("JBSWY3DPEHPK3PXP", "30;6", "TOTP Enabled")]
        [DataRow("JBSWY3DPEHPK3PXP", ";6", "Error, bad settings!")]
        [DataRow("JBSWY3DPEHPK3PXP", "30", "Error, bad settings!")]
        [DataRow("JBSWY3DPEHPK3PXP", null, "Error, storage!")]
        [DataRow("C5CYMIHWQUUZMKUGZHGEOSJSQDE4L===", "30;6", "Error, bad seed!")]
        [DataRow(null, "30;6", "Error, storage!")]
        [DataRow(null, null, "")]
        [DataTestMethod]
        public void GetCellData_ShouldReturnExpectedValues(string seed, string settings, string expected)
        {
            var column = new TrayTOTP_TOTPStatusColumn(_plugin);
            var pwEntry = new KeePassLib.PwEntry(true, true);
            if (seed != null)
            {
                var seedKey = _pluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed);
                pwEntry.Strings.Set(seedKey, new ProtectedString(false, seed));
            }
            if (settings != null)
            {
                var settingsKey = _pluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings);
                pwEntry.Strings.Set(settingsKey, new ProtectedString(false, settings));
            }

            var actual = column.GetCellData("columnName", pwEntry);

            actual.Should().Be(expected);
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
