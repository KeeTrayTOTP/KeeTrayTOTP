using System.Collections.Generic;
using System.Drawing;
using KeeTrayTOTP.Libraries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class TrayContextMenuPositionTests
    {
        [DataTestMethod]
        [DynamicData(nameof(DropDownLocationTestData))]
        public void CalculatedLocationForDropDown_Should_ReturnTheCorrectLocation(Size controlSize, Point mousePosition,
            Rectangle screenRect, Point expectedPoint)
        {
            var dropDownLocationCalculator = new DropDownLocationCalculator(controlSize);
            var sut = dropDownLocationCalculator.CalculateLocationForDropDown(mousePosition, screenRect);

            Assert.AreEqual(expectedPoint, sut);
        }

        private static IEnumerable<object[]> DropDownLocationTestData
        {
            get
            {
                var controlSize = new Size(300, 300);
                var screenRect = new Rectangle(0, 0, 1920, 1080);

                // top-left
                yield return new object[]
                {
                    controlSize, new Point(400, 500), screenRect,
                    // expected
                    new Point(400 - controlSize.Width, 500 - controlSize.Height)
                };

                // top-right
                yield return new object[]
                {
                    controlSize, new Point(200, 500), screenRect,
                    // expected
                    new Point(200, 500 - controlSize.Height)
                };

                // bottom-right
                yield return new object[]
                {
                    controlSize, new Point(200, 200), screenRect,
                    // expected
                    new Point(200, 200)
                };

                // bottom-left
                yield return new object[]
                {
                    controlSize, new Point(400, 200), screenRect,
                    // expected
                    new Point(400 - controlSize.Width, 200)
                };

                // top-left
                yield return new object[]
                {
                    controlSize, new Point(300, 300), screenRect,
                    // expected
                    new Point(0, 0)
                };

                // top-left
                yield return new object[]
                {
                    controlSize, new Point(screenRect.Width, screenRect.Height), screenRect,
                    // expected
                    new Point(screenRect.Width - controlSize.Width, screenRect.Height - controlSize.Height)
                };

                // edge case control size bigger than screen size
                yield return new object[]
                {
                    controlSize, new Point(0, 0), new Rectangle(0, 0, 200, 200),
                    // expected
                    new Point(0, 200 - controlSize.Height)
                };

                // edge case control height bigger than screen height
                yield return new object[]
                {
                    controlSize, new Point(50, 0), new Rectangle(0, 0, 1920, 200),
                    // expected
                    new Point(50, 200 - controlSize.Height)
                };

                // edge case control width bigger than screen width
                yield return new object[]
                {
                    controlSize, new Point(0, 0), new Rectangle(0, 0, 200, 1080),
                    // expected
                    new Point(0, 0)
                };

                // multi monitor test
                yield return new object[]
                {
                    controlSize, new Point(2500, 500), screenRect,
                    // expected
                    new Point(2500 - controlSize.Width, 500 - controlSize.Height)
                };

                // multi monitor test control bigger than screen
                yield return new object[]
                {
                    controlSize, new Point(1200, 150), new Rectangle(1080, 0, 200, 200),
                    // expected
                    new Point(1080, 200 - controlSize.Height)
                };
            }
        }
    }
}