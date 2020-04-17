using System;
using System.Windows.Forms;
using KeePass.Plugins;

namespace KeeTrayTOTP.Menu
{
    public class MenuItemProvider : IDisposable
    {
        private readonly MenuItemProviderBase _trayMenuItemProvider;
        private readonly MenuItemProviderBase _entryMenuItemProvider;
        private readonly MenuItemProviderBase _mainMenuItemProvider;

        public MenuItemProvider(KeeTrayTOTPExt plugin, IPluginHost pluginHost)
        {
            if (pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_LegacyTrayMenuProvider_Enable, false))
            {
                _trayMenuItemProvider = new LegacyTrayMenuItemProvider(plugin, pluginHost);
            }
            else
            {
                _trayMenuItemProvider = new TrayMenuItemProvider(plugin, pluginHost);
            }

            _entryMenuItemProvider = new EntryMenuItemProvider(plugin, pluginHost);
            _mainMenuItemProvider = new MainMenuItemProvider(plugin);
        }

        internal ToolStripMenuItem GetMenuItem(PluginMenuType type)
        {
            switch (type)
            {
                // Provide a menu item for the main location(s)
                case PluginMenuType.Main:
                    return ProvideMainMenuItem();

                case PluginMenuType.Entry:
                    return ProvideEntryMenuItem();

                case PluginMenuType.Tray:
                    return ProvideTrayMenuItem();

                case PluginMenuType.Group:
                    return ProvideGroupMenuItem();
                default:
                    return null; // No menu items in other locations
            }
        }

        protected ToolStripMenuItem ProvideTrayMenuItem()
        {
            return _trayMenuItemProvider != null ? _trayMenuItemProvider.ProvideMenuItem() : null;
        }

        protected ToolStripMenuItem ProvideEntryMenuItem()
        {
            return _entryMenuItemProvider != null ? _entryMenuItemProvider.ProvideMenuItem() : null;
        }

        protected ToolStripMenuItem ProvideMainMenuItem()
        {
            return _mainMenuItemProvider != null ? _mainMenuItemProvider.ProvideMenuItem() : null;
        }

        protected ToolStripMenuItem ProvideGroupMenuItem()
        {
            return null;
        }

        public void Dispose()
        {
            _trayMenuItemProvider.Dispose();
            _entryMenuItemProvider.Dispose();
            _mainMenuItemProvider.Dispose();
        }
    }
}