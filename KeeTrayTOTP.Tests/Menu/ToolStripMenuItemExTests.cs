using FluentAssertions;
using KeePass.UI;
using KeeTrayTOTP.Menu;
using KeeTrayTOTP.Tests.Extensions;
using KeeTrayTOTP.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KeeTrayTOTP.Helpers;

namespace KeeTrayTOTP.Tests.Menu
{
    [TestClass]
    public class ToolStripMenuItemExTests
    {
        [TestMethod]
        public void ToolStripMenuItemEx_ShouldReturnHasDropDownItems_EvenIfEmpty()
        {
            var sut = new ToolStripMenuItemEx("foo", new Bitmap(1,1));
            sut.ForceDropDownArrow = true;

            sut.HasDropDownItems.Should()
                .BeTrue("because we want to display the dropdown arrow even if there are no items present");
        }

        [TestMethod]
        public void ToolStripMenuItemEx_ShouldHaveDefaultBehavior_IfForceDropDownArrowIsNotSet()
        {
            var sut = new ToolStripMenuItemEx("foo", new Bitmap(1, 1));

            sut.HasDropDownItems.Should()
                .BeFalse("because it's the default behavior");
        }

        [TestMethod]
        public void ToolStripMenuItemEx_ShouldHaveDefaultBehavior_IfForceDropDownArrowIsNotSet2()
        {
            var sut = new ToolStripMenuItemEx("foo", new Bitmap(1, 1));
            sut.DropDownItems.Add("bar");

            sut.HasDropDownItems.Should()
                .BeTrue("because it's the default behavior");
        }

    }
}