using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluentAssertions;
using KeePass.App.Configuration;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeeTrayTOTP.Menu;
using KeeTrayTOTP.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KeeTrayTOTP.Tests.Menu
{
    [TestClass]
    public class TrayMenuTests
    {
        private KeeTrayTOTPExt _plugin;
        private TrayMenuItemProvider _trayMenuItemProvider;

        [TestInitialize]
        public void Initialize()
        {
            _plugin = CreatePluginHostMock(out var host);
            _trayMenuItemProvider = new TrayMenuItemProvider(_plugin, host.Object);
        }

        [TestMethod]
        public void TrayMenuItemProvider_ShouldReturnANewDropDownMenuItem()
        {
            var sut = _trayMenuItemProvider.ProvideMenuItem();

            sut.Should().NotBeNull();
            sut.HasDropDownItems.Should().BeTrue();
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnCorrectMenuItem_IfNoDatabaseIsOpened()
        {
            var documents = new List<PwDocument>(new[]
            {
                new PwDocument()
            });

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(1);
            sut.First().Text.Should().Be(Localization.Strings.NoDatabaseIsOpened,
                "because, there is no open database. (Keepass always provides a (new) PwDocument, even if there is no database open.");
        }


        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnEntryMenuItems_IfThereIsOnlyASingleDatabase()
        {
            var documents = new List<PwDocument>(new[]
            {
                new PwDocument().New().WithNonTotpEntries(2).WithTotpEnabledEntries(4),
            });

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(4,
                "because, the items are added directly to the root tray menuitem if there is only a single database opened.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnCorrectInfo_IfThereAreNoTotpEntries()
        {
            var documents = new List<PwDocument>(new[]
            {
                new PwDocument().New().WithNonTotpEntries(4)
            });

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(1);
            sut.First().Text.Should().Contain(Localization.Strings.NoTOTPEntriesFound,
                "because, there were no totp entries found in the database.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnEntryMenuItems_IfThereIsAtLeast2Databases()
        {
            var documents = new List<PwDocument>(new[]
            {
                new PwDocument().New().WithNonTotpEntries(4).WithTotpEnabledEntries(4),
                new PwDocument().New().WithNonTotpEntries(4).WithTotpEnabledEntries(4),
            });

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(2,
                "because, the open databases are added as sub menu if there are at least 2 databases.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldCreateASingleMenuItemWithLocked_IfASingleLockedDatabasesIsPresent()
        {
            var documents = new List<PwDocument>(new[]
            {
                new PwDocument().New().Locked()
            });

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(1);
            sut.First().Text.Should().Contain("[" + Localization.Strings.Locked + "]", "because, there is only a locked database.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldCreateMenuItemsContainingLocked_IfMultipleDatabasesAreLocked()
        {
            var documents = new List<PwDocument>(new[]
            {
                new PwDocument().New().Locked(),
                new PwDocument().New().Locked()
            });

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(2);
            sut.Should().OnlyContain(s => s.Text.Contains("[" + Localization.Strings.Locked + "]"),
                "because, all databases are locked.");
        }

        private static KeeTrayTOTPExt CreatePluginHostMock(out Mock<IPluginHost> host)
        {
            var plugin = new KeeTrayTOTPExt();
            host = new Mock<IPluginHost>(MockBehavior.Strict);

            var mainForm = new MainForm();
            host.SetupGet(c => c.MainWindow).Returns(mainForm);

            var customConfig = new AceCustomConfig();
            host.SetupGet(c => c.CustomConfig).Returns(customConfig);

            var columnProviderPool = new ColumnProviderPool();
            host.SetupGet(c => c.ColumnProviderPool).Returns(columnProviderPool);

            plugin.Initialize(host.Object);

            return plugin;
        }
    }
}