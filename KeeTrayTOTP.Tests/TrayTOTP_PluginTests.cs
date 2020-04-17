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

        [TestMethod]
        public void InitializePlugin_ShouldAddTwoColumns()
        {
            var plugin = CreatePluginHostMock(out var host);
            var numberOfColumnsBeforeInitialize = host.Object.ColumnProviderPool.Count;

            plugin.Initialize(host.Object);

            host.Object.ColumnProviderPool.Should().HaveCount(numberOfColumnsBeforeInitialize + 2);
        }

        [TestMethod]
        public void Terminate_ShouldOnlyRemoveOurColumns()
        {
            var plugin = CreatePluginHostMock(out var host);

            // add a fake column that does not belong to our plugin
            var otherColumn = new Mock<ColumnProvider>().Object;
            host.Object.ColumnProviderPool.Add(otherColumn);

            plugin.Initialize(host.Object);
            plugin.Terminate();

            // only the fake column should be in the pool after the plugin terminates
            host.Object.ColumnProviderPool.Should().HaveCount(1);
            host.Object.ColumnProviderPool.Should().OnlyContain(c => c == otherColumn);
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
