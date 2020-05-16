using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KeeTrayTOTP.Helpers
{
    [DesignerCategory("")]
    public class ToolStripMenuItemEx : ToolStripMenuItem
    {
        public ToolStripMenuItemEx(string text, Image image) : base(text, image)
        {
        }

        public ToolStripMenuItemEx(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
        }

        /// <summary>
        /// This is required, because there is a bug in the .NET Framework, which calculates the position of dropdown menus wrong,
        /// if there is a multi-monitor setup with two monitors stacked above with the taskbar on the bottom of the top screen.
        /// </summary>
        protected override Point DropDownLocation
        {
            get
            {
                var dropDownLocation = base.DropDownLocation;
                
                var screenOfDropDown = Screen.FromPoint(dropDownLocation);
                var screenOfParentMenu = Screen.FromControl(Parent);

                if (!screenOfParentMenu.Equals(screenOfDropDown))
                {
                    dropDownLocation.Offset(0, -DropDown.Height);
                }

                return dropDownLocation;
            }
        }

        /// <summary>
        /// Controls whether the drop-down arrow is displayed even if there are no visible drop-down items
        /// </summary>
        public bool ForceDropDownArrow { get; set; }

        public override bool HasDropDownItems
        {
            get
            {
                return ForceDropDownArrow || base.HasDropDownItems;
            }
        }
    }
}