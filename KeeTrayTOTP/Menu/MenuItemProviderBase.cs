using System.Windows.Forms;

namespace KeeTrayTOTP.Menu
{
    public abstract class MenuItemProviderBase
    {
        public abstract ToolStripMenuItem ProvideMenuItem();

        public virtual void Dispose()
        {}
    }
}