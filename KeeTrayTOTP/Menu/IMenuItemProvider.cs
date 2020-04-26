using System.Windows.Forms;

namespace KeeTrayTOTP.Menu
{
    public interface IMenuItemProvider
    {
        ToolStripMenuItem ProvideMenuItem();
    }
}