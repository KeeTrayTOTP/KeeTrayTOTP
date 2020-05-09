using KeePass.App.Configuration;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using Moq;

namespace KeeTrayTOTP.Tests.Helpers
{
    public static class PluginHostHelper
    {
        public static (KeeTrayTOTPExt plugin, Mock<IPluginHost> pluginHost) Create()
        {
            var plugin = new KeeTrayTOTPExt();
            var pluginHost = new Mock<IPluginHost>(MockBehavior.Strict);

            pluginHost.SetupGet(c => c.MainWindow).Returns(new MainForm());
            pluginHost.SetupGet(c => c.CustomConfig).Returns(new AceCustomConfig());
            pluginHost.SetupGet(c => c.ColumnProviderPool).Returns(new ColumnProviderPool());

            return (plugin, pluginHost);
        }

        public static (KeeTrayTOTPExt plugin, Mock<IPluginHost> pluginHost) CreateAndInitialize()
        {
            var createResult = Create();
            createResult.plugin.Initialize(createResult.pluginHost.Object);

            return createResult;
        }
    }
}
