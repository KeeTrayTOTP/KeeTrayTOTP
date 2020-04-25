using System;
using System.Windows.Forms;
using KeePass.Plugins;

namespace KeeTrayTOTP.Menu
{
    public class MenuItemProvider : IDisposable
    {
        internal MenuItemProviderBase TrayMenuItemProvider { get; private set; }
        internal MenuItemProviderBase EntryMenuItemProvider { get; private set; }
        internal MenuItemProviderBase MainMenuItemProvider { get; private set; }

        public MenuItemProvider(KeeTrayTOTPExt plugin, IPluginHost pluginHost)
        {
            if (pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_LegacyTrayMenuProvider_Enable, false))
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
            return TrayMenuItemProvider != null ? TrayMenuItemProvider.ProvideMenuItem() : null;
        }

        protected ToolStripMenuItem ProvideEntryMenuItem()
        {
            return EntryMenuItemProvider != null ? EntryMenuItemProvider.ProvideMenuItem() : null;
        }

        protected ToolStripMenuItem ProvideMainMenuItem()
        {
            return MainMenuItemProvider != null ? MainMenuItemProvider.ProvideMenuItem() : null;
        }

        protected ToolStripMenuItem ProvideGroupMenuItem()
        {
            return null;
        }

        public void Dispose()
        {
            TrayMenuItemProvider.Dispose();
            EntryMenuItemProvider.Dispose();
            MainMenuItemProvider.Dispose();
        }
    }
}