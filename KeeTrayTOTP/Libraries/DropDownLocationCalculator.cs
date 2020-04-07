using System.Drawing;
using System.Windows.Forms;

namespace KeeTrayTOTP.Libraries
{
    /// <summary>
    /// Calculator for the location of a dropdown control based on the location of the mouse and screen space available
    /// </summary>
    public class DropDownLocationCalculator
    {
        private Size _controlSize;

        /// <summary>
        ///     Initializes a calculator which can calculate the location of a dropdown control based on the location of the mouse
        ///     and the screen space available.
        /// </summary>
        /// <param name="controlSize">The size of the control which will be dropdown-ed</param>
        public DropDownLocationCalculator(Size controlSize)
        {
            _controlSize = controlSize;
        }

        /// <summary>
        ///     Calculate the position for a dropdown control based on the given mouse position.
        ///     The default behavior is to calculate the location for opening the dropdown to the top-left.
        /// </summary>
        /// <param name="mousePosition">The position in which the control will be anchored</param>
        /// <returns>The new location for the control</returns>
        internal Point CalculateLocationForDropDown(Point mousePosition)
        {
            var screen = Screen.FromPoint(mousePosition);
            return CalculateLocationForDropDown(mousePosition, screen.Bounds);
        }

        /// <summary>
        ///     Calculate the position for a dropdown control based on the given mouse position and screen rectangle
        ///     The default behavior is to calculate the location for opening the dropdown to the top-left.
        /// </summary>
        /// <param name="mousePosition">The position in which the control will be anchored</param>
        /// <param name="screenRect">
        ///     The screen rectangle which is used for the calculation. This should be the screen on which the
        ///     control will be opened
        /// </param>
        /// <returns></returns>
        internal Point CalculateLocationForDropDown(Point mousePosition, Rectangle screenRect)
        {
            var screenHeight = screenRect.Height;
            var screenWidth = screenRect.Width;

            // after calculating the offset, the mouse position is relative to the screen containing the cursor
            // this allows for correct calculations even on multi-monitor systems
            mousePosition.Offset(-screenRect.X, -screenRect.Y);

            int calculatedX = CalculateX(mousePosition, screenWidth);
            int calculatedY = CalculateY(mousePosition, screenHeight);

            return new Point(calculatedX + screenRect.X, calculatedY + screenRect.Y);
        }

        private int CalculateY(Point mousePosition, int screenHeight)
        {
            int calculatedY;
            if (IsEnoughSpaceToTheTop(mousePosition))
            {
                calculatedY = mousePosition.Y - _controlSize.Height;
            }
            else if (IsEnoughSpaceToTheBottom(mousePosition, screenHeight))
            {
                calculatedY = mousePosition.Y;
            }
            else
            {
                // the control height is higher than the screen height
                calculatedY = screenHeight - _controlSize.Height;
            }

            return calculatedY;
        }

        private int CalculateX(Point mousePosition, int screenWidth)
        {
            int calculatedX;
            if (IsEnoughSpaceToTheLeft(mousePosition))
            {
                calculatedX = mousePosition.X - _controlSize.Width;
            }
            else if (IsEnoughSpaceToTheRight(mousePosition, screenWidth))
            {
                calculatedX = mousePosition.X;
            }
            else
            {
                // the control width is bigger than the screen width
                calculatedX = 0;
            }

            return calculatedX;
        }

        private bool IsEnoughSpaceToTheLeft(Point mousePosition)
        {
            return (mousePosition.X - _controlSize.Width) >= 0;
        }

        private bool IsEnoughSpaceToTheTop(Point mousePosition)
        {
            return (mousePosition.Y - _controlSize.Height) >= 0;
        }

        private bool IsEnoughSpaceToTheRight(Point mousePosition, int screenWidth)
        {
            return (mousePosition.X + _controlSize.Width) <= screenWidth;
        }

        private bool IsEnoughSpaceToTheBottom(Point mousePosition, int screenHeight)
        {
            return (mousePosition.Y + _controlSize.Height) <= screenHeight;
        }
    }
}