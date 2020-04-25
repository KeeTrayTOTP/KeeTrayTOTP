using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeeTrayTOTP.Helpers
{
    internal static class MenuItemHelper
    {
        /// <summary>
        /// Prevent ToolStripMenuItems from jumping to second screen
        /// see: https://stackoverflow.com/a/49626530/1627022
        /// </summary>
        internal static void OnDatabaseDropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null || !menuItem.HasDropDownItems)
            {
                return; // not a drop down item
            }

            //get position of current menu item
            var pos = new Point(menuItem.GetCurrentParent().Left, menuItem.GetCurrentParent().Top);

            // Current bounds of the current monitor
            Screen currentScreen = Screen.FromPoint(pos);

            // Find the width of sub-menu
            int maxWidth = 0;
            foreach (var subItem in menuItem.DropDownItems)
            {
                if (subItem.GetType() == typeof(ToolStripMenuItem))
                {
                    var mnu = (ToolStripMenuItem)subItem;
                    maxWidth = Math.Max(mnu.Width, maxWidth);
                }
            }

            maxWidth += 10; // Add a little wiggle room

            int farRight = pos.X + menuItem.Width + maxWidth;
            int farLeft = pos.X - maxWidth;

            //get left and right distance to compare
            int leftGap = farLeft - currentScreen.Bounds.Left;
            int rightGap = currentScreen.Bounds.Right - farRight;

            menuItem.DropDownDirection = leftGap >= rightGap ? ToolStripDropDownDirection.Left : ToolStripDropDownDirection.Right;
        }
    }
}
