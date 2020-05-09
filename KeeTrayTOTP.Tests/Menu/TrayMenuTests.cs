using FluentAssertions;
using KeePass.UI;
using KeeTrayTOTP.Menu;
using KeeTrayTOTP.Tests.Extensions;
using KeeTrayTOTP.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace KeeTrayTOTP.Tests.Menu
{
    [TestClass]
    public class TrayMenuTests
    {
        private TrayMenuItemProvider _trayMenuItemProvider;

        [TestInitialize]
        public void Initialize()
        {
            var (plugin, host) = PluginHostHelper.CreateAndInitialize();

            _trayMenuItemProvider = new TrayMenuItemProvider(plugin, host.Object);
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
            var pwDocument = new PwDocument();

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(pwDocument.AsList()).ToList();

            sut.Count.Should().Be(1);
            sut.First().Text.Should().Be(Localization.Strings.NoDatabaseIsOpened,
                "because, there is no open database. (KeePass always provides a (new) PwDocument, even if there is no database open.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnEntryMenuItems_IfThereIsOnlyASingleDatabase()
        {
            var pwDocument = new PwDocument().New().WithNonTotpEntries(2).WithTotpEnabledEntries(4);

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(pwDocument.AsList()).ToList();

            sut.Count.Should().Be(4,
                "because, the items are added directly to the root tray menuitem if there is only a single database opened.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnCorrectInfo_IfThereAreNoTotpEntries()
        {
            var pwDocument = new PwDocument().New().WithNonTotpEntries(4);

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(pwDocument.AsList()).ToList();

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
                new PwDocument().New().WithNonTotpEntries(4).WithTotpEnabledEntries(4)
            });

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(2,
                "because, the open databases are added as sub menu if there are at least 2 databases.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldCreateASingleMenuItemWithLocked_IfASingleLockedDatabasesIsPresent()
        {
            var pwDocument = new PwDocument().New().Locked();

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(pwDocument.AsList()).ToList();

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

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldCreateDisabledMenuItems_IfTotpSettingsNotValid()
        {
            var pwDocument = new PwDocument().New().WithFaultyTotpEnabledEntries(2);

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(pwDocument.AsList()).ToList();

            sut.Count.Should().Be(2);
            sut.Should().OnlyContain(s => !s.Enabled,
                "because all entries contain invalid settings and can't be used");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnEntryMenuItemsNotInRecycleBin()
        {
            var pwDocument = new PwDocument().New().WithNonTotpEntries(2).WithTotpEnabledEntries(4).WithDeletedTotpEnabledEntries(2);

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(pwDocument.AsList()).ToList();

            sut.Count.Should().Be(4,
                "because, valid entries in the recycle bin should not show up.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnEntryMenuItems_WhenRecycleBinNotEnabled()
        {
            var pwDocument = new PwDocument().New().WithNonTotpEntries(2).WithTotpEnabledEntries(4).WithDeletedTotpEnabledEntries(2);

            // Treat recycle bin as a regular folder
            pwDocument.Database.RecycleBinEnabled = false;

            var sut = _trayMenuItemProvider.BuildMenuItemsForRootDropDown(pwDocument.AsList()).ToList();

            sut.Count.Should().Be(6,
                "because, the recycle bin is treated as a regular folder.");
        }
    }
}