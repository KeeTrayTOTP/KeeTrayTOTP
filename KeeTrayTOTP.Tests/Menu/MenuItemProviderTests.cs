using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluentAssertions;
using KeePass.App.Configuration;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeeTrayTOTP.Menu;
using KeeTrayTOTP.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KeeTrayTOTP.Tests.Menu
{
    [TestClass]
    public class MenuItemProviderTests
    {
        [TestMethod]
        public void MenuItemProvider_ShouldDefaultReturnTheTrayMenuItemProvider()
        {
            var plugin = CreatePluginHostMock(out var host);
            plugin.Initialize(host.Object);

            var sut = new MenuItemProvider(plugin, host.Object);

            sut.TrayMenuItemProvider.Should().BeOfType<TrayMenuItemProvider>();
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheLegacyTrayMenuItemProvider_IfSetInConfig()
        {
            var plugin = CreatePluginHostMock(out var host);
            host.Object.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_LegacyTrayMenuProvider_Enable, true);
            plugin.Initialize(host.Object);

            var sut = new MenuItemProvider(plugin, host.Object);
            sut.TrayMenuItemProvider.Should().BeOfType<LegacyTrayMenuItemProvider>();
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheCorrectTrayMenuEntries()
        {
            var plugin = CreatePluginHostMock(out var host);
            plugin.Initialize(host.Object);

            var sut = new MenuItemProvider(plugin, host.Object);

            var trayMenuItem = sut.GetMenuItem(PluginMenuType.Tray);
            trayMenuItem.Should().NotBeNull();
            trayMenuItem.HasDropDownItems.Should().BeTrue();
            trayMenuItem.DropDownItems.Should().HaveCount(1, "because, there should be a pseudo entry.");
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheCorrectMainMenuEntries()
        {
            var plugin = CreatePluginHostMock(out var host);
            plugin.Initialize(host.Object);

            var sut = new MenuItemProvider(plugin, host.Object);

            var mainMenuItem = sut.GetMenuItem(PluginMenuType.Main);
            mainMenuItem.Should().NotBeNull();
            mainMenuItem.HasDropDownItems.Should().BeTrue();
            mainMenuItem.DropDownItems.Should().HaveCount(4);
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheCorrectEntryMenuEntries()
        {
            var plugin = CreatePluginHostMock(out var host);
            plugin.Initialize(host.Object);

            var sut = new MenuItemProvider(plugin, host.Object);

            var mainMenuItem = sut.GetMenuItem(PluginMenuType.Entry);
            mainMenuItem.Should().NotBeNull();
            mainMenuItem.HasDropDownItems.Should().BeTrue();
            mainMenuItem.DropDownItems.Should().HaveCount(3);
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnNullForGroupMenuEntries()
        {
            var plugin = CreatePluginHostMock(out var host);
            plugin.Initialize(host.Object);

            var sut = new MenuItemProvider(plugin, host.Object);

            var mainMenuItem = sut.GetMenuItem(PluginMenuType.Group);
            mainMenuItem.Should().BeNull();
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnNullForUnknownEnumValue()
        {
            var plugin = CreatePluginHostMock(out var host);
            plugin.Initialize(host.Object);

            var sut = new MenuItemProvider(plugin, host.Object);

            var mainMenuItem = sut.GetMenuItem((PluginMenuType)4);
            mainMenuItem.Should().BeNull();
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