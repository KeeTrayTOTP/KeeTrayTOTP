using FluentAssertions;
using KeePass.Plugins;
using KeeTrayTOTP.Menu;
using KeeTrayTOTP.Tests.Helpers;
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
            (_plugin, _host) = PluginHostHelper.Create();
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
            _plugin.Initialize(_host.Object);
            _plugin.Settings.LegacyTrayMenuProviderEnable = true;

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
            trayMenuItem.DropDownItems.Count.Should().Be(0, "because, the entries are added at opening of the menu.");
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheCorrectMainMenuEntries()
        {
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

            var mainMenuItem = sut.GetMenuItem(PluginMenuType.Main);
            mainMenuItem.Should().NotBeNull();
            mainMenuItem.HasDropDownItems.Should().BeTrue();
            mainMenuItem.DropDownItems.Count.Should().Be(4);
        }

        [TestMethod]
        public void MenuItemProvider_ShouldReturnTheCorrectEntryMenuEntries()
        {
            _plugin.Initialize(_host.Object);

            var sut = new MenuItemProvider(_plugin, _host.Object);

            var mainMenuItem = sut.GetMenuItem(PluginMenuType.Entry);
            mainMenuItem.Should().NotBeNull();
            mainMenuItem.HasDropDownItems.Should().BeTrue();
            mainMenuItem.DropDownItems.Count.Should().Be(3);
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
    }
}