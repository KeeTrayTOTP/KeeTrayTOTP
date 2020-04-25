using System;
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
        [TestMethod]
        public void TrayMenuItemProvider_ShouldReturnANewDropDownMenuItem()
        {
            var plugin = CreatePluginHostMock(out var host);
            var trayMenuItemProvider = new TrayMenuItemProvider(plugin, host.Object);

            var sut = trayMenuItemProvider.ProvideMenuItem();

            sut.Should().NotBeNull();
            sut.HasDropDownItems.Should().BeTrue();
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnCorrectMenuItemIfNoDatabaseIsOpened()
        {
            var plugin = CreatePluginHostMock(out var host);
            var trayMenuItemProvider = new TrayMenuItemProvider(plugin, host.Object);
            var documents = new List<PwDocument>(new[]
            {
                new PwDocument() // 
            });

            var sut = trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(1);
            sut.First().Text.Should().Be(Localization.Strings.NoDatabaseIsOpened, "because, there is no open database. (Keepass always provides a (new) PwDocument, even if there is no database open.");
        }


        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnEntryMenuItems_WhenThereIsOnlyASingleDatabase()
        {
            var plugin = CreatePluginHostMock(out var host);
            var trayMenuItemProvider = new TrayMenuItemProvider(plugin, host.Object);
            var documents = new List<PwDocument>(new[]
            {
                CreateSamplePwDocument(4,2),
            });

            var sut = trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(4, "because, the items are added directly to the root tray menuitem if there is only a single database opened.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldReturnEntryMenuItems_WhenThereIsAtLeast2Databases()
        {
            var plugin = CreatePluginHostMock(out var host);
            var trayMenuItemProvider = new TrayMenuItemProvider(plugin, host.Object);
            var documents = new List<PwDocument>(new[]
            {
                CreateSamplePwDocument(4,4),
                CreateSamplePwDocument(4,4),
            });

            var sut = trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(2, "because, the open databases are added as sub menu if there are at least 2 databases.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldDisplayASingleLockedDatabases()
        {
            var plugin = CreatePluginHostMock(out var host);
            var trayMenuItemProvider = new TrayMenuItemProvider(plugin, host.Object);

            var documents = new List<PwDocument>(new[]
            {
                CreateSamplePwDocument().Lock()
            });

            var sut = trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(1);
            sut.First().Text.Should().Contain("[" + Localization.Strings.Locked + "]", "because, there is only a locked database.");
        }

        [TestMethod]
        public void BuildMenuItemsForRootDropDown_ShouldDisplayMultipleDatabasesAsLocked()
        {
            var plugin = CreatePluginHostMock(out var host);
            var trayMenuItemProvider = new TrayMenuItemProvider(plugin, host.Object);
            var documents = new List<PwDocument>(new[]
            {
                CreateSamplePwDocument().Lock(),
                CreateSamplePwDocument().Lock(),
            });

            var sut = trayMenuItemProvider.BuildMenuItemsForRootDropDown(documents).ToList();

            sut.Count.Should().Be(2);
            sut.Should().OnlyContain(s => s.Text.Contains("[" + Localization.Strings.Locked + "]"), "because, all databases are locked.");
        }

        private static PwDocument CreateSamplePwDocument(int totpEnabledEntriesCount = 1, int totpDisabledEntriesCount = 1)
        {
            PwDocument pwDocument = new PwDocument();
            pwDocument.Database.New(IOConnectionInfo.FromPath("temp"), new CompositeKey());

            for (int i = 0; i < totpEnabledEntriesCount; i++)
            {
                pwDocument.Database.RootGroup.AddEntry(
                    CreateTOTPEnabledPwEntry(),
                    true);
            }

            for (int i = 0; i < totpDisabledEntriesCount; i++)
            {
                pwDocument.Database.RootGroup.AddEntry(
                    new PwEntry(true, true),
                    true);
            }

            return pwDocument;
        }

        private static PwEntry CreateTOTPEnabledPwEntry()
        {
            PwEntry pwEntry = new PwEntry(true, true);
            pwEntry.Strings.Set(Localization.Strings.TOTPSeed, new ProtectedString(false, "JBSWY3DPEHPK3PXP"));
            pwEntry.Strings.Set(Localization.Strings.TOTPSettings, new ProtectedString(false, "30;6"));
            return pwEntry;
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
