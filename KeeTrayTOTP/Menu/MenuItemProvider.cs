using System;
using System.Linq;
using System.Windows.Forms;
using KeePass.Plugins;

namespace KeeTrayTOTP.Menu
{
    public sealed class MenuItemProvider : IDisposable
    {
        private bool _isDisposed;
        internal TrayMenuItemProvider TrayMenuItemProvider { get; private set; }
        internal EntryMenuItemProvider EntryMenuItemProvider { get; private set; }
        internal MainMenuItemProvider MainMenuItemProvider { get; private set; }

        public MenuItemProvider(KeeTrayTOTPExt plugin, IPluginHost pluginHost)
        {
            if (plugin.Settings.LegacyTrayMenuProviderEnable)
            {
                TrayMenuItemProvider = new LegacyTrayMenuItemProvider(plugin, pluginHost);
            }
            else
            {
                TrayMenuItemProvider = new TrayMenuItemProvider(plugin, pluginHost);
            }

            EntryMenuItemProvider = new EntryMenuItemProvider(plugin, pluginHost);
            MainMenuItemProvider = new MainMenuItemProvider(plugin);
        }

        internal ToolStripMenuItem GetMenuItem(PluginMenuType type)
        {
            switch (type)
            {
                // Provide a menu item for the main location(s)
                case PluginMenuType.Main:
                    return MainMenuItemProvider.ProvideMenuItem();

                case PluginMenuType.Entry:
                    return EntryMenuItemProvider.ProvideMenuItem();

                case PluginMenuType.Tray:
                    return TrayMenuItemProvider.ProvideMenuItem();

                default:
                    return null; // No menu items in other locations
            }
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                var disposableTypes =
                    new IMenuItemProvider[] {TrayMenuItemProvider, EntryMenuItemProvider, MainMenuItemProvider}
                        .OfType<IDisposable>();

                foreach (var disposable in disposableTypes)
                {
                    disposable.Dispose();
                }
            }

            _isDisposed = true;
        }
    }
}