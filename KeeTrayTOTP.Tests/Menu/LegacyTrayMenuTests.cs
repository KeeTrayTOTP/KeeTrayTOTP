﻿using FluentAssertions;
using KeePass.App.Configuration;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeeTrayTOTP.Menu;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KeeTrayTOTP.Tests.Menu
{
    [TestClass]
    public class LegacyTrayMenuTests
    {
        [TestMethod]
        public void LegacyTrayMenuItemProvider_ShouldReturnNull()
        {
            var plugin = CreatePluginHostMock(out var host);
            var legacyTrayMenuItemProvider = new LegacyTrayMenuItemProvider(plugin, host.Object);

            var sut = legacyTrayMenuItemProvider.ProvideMenuItem();

            sut.Should().BeNull();
        }

        [TestMethod]
        public void LegacyTrayMenuItemProvider_ShouldAddItemsDirectlyToMainWindowsTrayContextMenu()
        {
            var plugin = CreatePluginHostMock(out var host);
            host.Object.CustomConfig.SetBool("traymenulegacymenuprovider_enable", true);
            var oldItemCount = host.Object.MainWindow.TrayContextMenu.Items.Count;
            
            plugin.Initialize(host.Object);

            var sut = host.Object.MainWindow.TrayContextMenu.Items.Count;

            sut.Should().Be(oldItemCount + 2);
        }

        private static KeeTrayTOTPExt CreatePluginHostMock(out Mock<IPluginHost> host)
        {
            var plugin = new KeeTrayTOTPExt();
            host = new Mock<IPluginHost>(MockBehavior.Strict);

            var mainForm = new MainForm();
            host.SetupGet(c => c.MainWindow).Returns(mainForm);

            var customConfig = new AceCustomConfig();
            host.SetupGet(c => c.CustomConfig).Returns(customConfig);

            var columnProviderPool = new ColumnProviderPool();
            host.SetupGet(c => c.ColumnProviderPool).Returns(columnProviderPool);

            return plugin;
        }
    }
}
