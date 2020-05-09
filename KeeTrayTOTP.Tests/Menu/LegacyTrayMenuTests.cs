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

            sut.Should().BeNull();
        }

        [TestMethod]
        public void LegacyTrayMenuItemProvider_ShouldAddItemsDirectlyToMainWindowsTrayContextMenu()
        {
            var (plugin, host) = PluginHostHelper.Create();
            host.Object.CustomConfig.SetBool("traymenulegacymenuprovider_enable", true);
            var oldItemCount = host.Object.MainWindow.TrayContextMenu.Items.Count;

            plugin.Initialize(host.Object);

            var sut = host.Object.MainWindow.TrayContextMenu.Items.Count;

            sut.Should().Be(oldItemCount + 2);
        }
    }
}
