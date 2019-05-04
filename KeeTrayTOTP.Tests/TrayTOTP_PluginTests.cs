using System;
using FluentAssertions;
using KeePass.App.Configuration;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class TrayTOTP_PluginTests
    {
        [TestMethod]
        public void InitializePlugin_ShouldNotThrow()
        {
            var plugin = CreatePluginHostMock(out var host);

            Action act = () => plugin.Initialize(host.Object);

            act.Should().NotThrow();
        }

        private static KeeTrayTOTPExt CreatePluginHostMock(out Mock<IPluginHost> host)
        {
            var plugin = new KeeTrayTOTPExt();
            host = new Mock<IPluginHost>(MockBehavior.Strict);

            var keepassForm = new MainForm();
            host.SetupGet(c => c.MainWindow).Returns(keepassForm);

            var customConfig = new AceCustomConfig();
            host.SetupGet(c => c.CustomConfig).Returns(customConfig);

            var columnProviderPool = new ColumnProviderPool();
            host.SetupGet(c => c.ColumnProviderPool).Returns(columnProviderPool);

            return plugin;
        }
    }
}
