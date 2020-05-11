using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentAssertions;
using KeePass.UI;
using KeePass.Util;
using KeeTrayTOTP.Menu;
using KeeTrayTOTP.Tests.Extensions;
using KeeTrayTOTP.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeeTrayTOTP.Tests.Menu
{
    [TestClass]
    public class EntryMenuItemProviderTests
    {
        private EntryMenuItemProvider _entryMenuItemProvider;

        [TestInitialize]
        public void Initialize()
        {
            var (plugin, host) = PluginHostHelper.CreateAndInitialize();

            _entryMenuItemProvider = new EntryMenuItemProvider(plugin, host.Object);
        }

        [TestMethod]
        public void EntryMenuItemProvider_Should_ProvideAllMenuItemsWithCorrectShortcutKeys()
        {
            var sut = _entryMenuItemProvider.ProvideMenuItem();

            sut.DropDownItems.Count.Should().Be(3,
                "because, there are exactly three menu items for an entry.");

            sut.DropDownItems
                .Cast<ToolStripMenuItem>()
                .All(x => x.ShortcutKeys != Keys.None)
                .Should().BeTrue("because, all entries should have a shortcut key assigned.");
        }
    }
}
