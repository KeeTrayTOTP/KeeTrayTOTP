using FluentAssertions;
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
    public class MenuItemProviderTests
    {
        private KeeTrayTOTPExt _plugin;
        private Mock<IPluginHost> _host;

        [TestInitialize]
        public void Initialize()
        {
            _plugin = CreatePluginHostMock(out var host);
            _host = host;
        }

        [TestMethod]
        public void MenuItemProvider_ShouldDefaultReturnTheTrayMenuItemProvider()
        {
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

            sut.TrayMenuItemProvider.Should().BeOfType<TrayMenuItemProvider>();
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheLegacyTrayMenuItemProvider_IfSetInConfig()
        {
            _host.Object.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_LegacyTrayMenuProvider_Enable, true);
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

            sut.TrayMenuItemProvider.Should().BeOfType<LegacyTrayMenuItemProvider>();
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheCorrectTrayMenuEntries()
        {
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

            var trayMenuItem = sut.GetMenuItem(PluginMenuType.Tray);

            trayMenuItem.Should().NotBeNull();
            trayMenuItem.HasDropDownItems.Should().BeTrue();
            trayMenuItem.DropDownItems.Should().HaveCount(1, "because, there should be a pseudo entry.");
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheCorrectMainMenuEntries()
        {
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

            var mainMenuItem = sut.GetMenuItem(PluginMenuType.Main);
            mainMenuItem.Should().NotBeNull();
            mainMenuItem.HasDropDownItems.Should().BeTrue();
            mainMenuItem.DropDownItems.Should().HaveCount(4);
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheCorrectEntryMenuEntries()
        {
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

            var mainMenuItem = sut.GetMenuItem(PluginMenuType.Entry);
            mainMenuItem.Should().NotBeNull();
            mainMenuItem.HasDropDownItems.Should().BeTrue();
            mainMenuItem.DropDownItems.Should().HaveCount(3);
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnNullForGroupMenuEntries()
        {
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

            var mainMenuItem = sut.GetMenuItem(PluginMenuType.Group);
            mainMenuItem.Should().BeNull();
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnNullForUnknownEnumValue()
        {
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

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