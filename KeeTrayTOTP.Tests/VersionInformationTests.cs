using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class VersionInformationTests
    {
        private readonly string[] VersionInformationLines = File.ReadAllLines("version_manifest.txt");

        [TestMethod]
        public void VersionFile_ShouldHaveCorrectLength()
        {
            VersionInformationLines.Should().HaveCountGreaterOrEqualTo(3, "File should be a minimum of 3 lines");
        }

        [TestMethod]
        public void VersionFile_ShouldNotContainEmptyLines()
        {
            VersionInformationLines.Should().NotContain("", "File should not contain empty lines");
        }

        [TestMethod]
        public void VersionFile_ShouldStartAndEndWithSeparators()
        {
            var separator = VersionInformationLines.First();
            separator.Should().HaveLength(1);
            VersionInformationLines.Last().Should().Be(separator, "File should start and end with the same separator");
        }

        [TestMethod]
        public void VersionFile_ShouldContainSingleEntryForPluginWithCurrentVersion()
        {
            var separatorArray = new[] { VersionInformationLines.First() };
            var pluginLines = VersionInformationLines.Except(separatorArray);
            pluginLines.Should().HaveCount(1);

            var updateParts = pluginLines.Single().Split(separatorArray, System.StringSplitOptions.RemoveEmptyEntries);
            updateParts.Should().HaveCount(2);

            var assembly = typeof(KeeTrayTOTPExt).Assembly;
            var assemblyFileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var assemblyTitleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
           
            updateParts.First().Should().Be(assemblyTitleAttribute.Title, "First part should equal the plugin name");
            updateParts.Last().Should().Be(assemblyFileVersionAttribute.Version, "Last part should equal the current plugin version.");
        }
    }
}
