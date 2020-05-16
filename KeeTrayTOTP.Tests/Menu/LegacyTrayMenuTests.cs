using FluentAssertions;
using KeeTrayTOTP.Menu;
using KeeTrayTOTP.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeeTrayTOTP.Tests.Menu
{
    [TestClass]
    public class LegacyTrayMenuTests
    {
        [TestMethod]
        public void LegacyTrayMenuItemProvider_ShouldReturnNull()
        {
            var (plugin, host) = PluginHostHelper.Create();
            var legacyTrayMenuItemProvider = new LegacyTrayMenuItemProvider(plugin, host.Object);

            var sut = legacyTrayMenuItemProvider.ProvideMenuItem();

            sut.Should().BeNull("because we do not provide an official tray menu item in legacy mode.");
        }

        [TestMethod]
        public void LegacyTrayMenuItemProvider_ShouldAddItemsDirectlyToMainWindowsTrayContextMenu()
        {
            var (plugin, host) = PluginHostHelper.Create();
            host.Object.CustomConfig.SetBool("traymenulegacymenuprovider_enable", true);
            var oldItemCount = host.Object.MainWindow.TrayContextMenu.Items.Count;

            plugin.Initialize(host.Object);

            var sut = host.Object.MainWindow.TrayContextMenu.Items.Count;

            sut.Should().Be(oldItemCount + 2, "because we inject two menu items into the official KeePass tray menu");
        }
    }
}
