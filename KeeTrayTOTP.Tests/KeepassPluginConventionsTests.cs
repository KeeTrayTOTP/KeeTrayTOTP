using FluentAssertions;
using KeePass.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace KeeTrayTOTP.Tests
{
    /// <summary>
    /// Validate Keepass Plugin Conventions
    /// https://keepass.info/help/v2_dev/plg_index.html
    /// </summary>
    [TestClass]
    public class KeepassPluginConventionsTests
    {
        /// <summary>
        /// Product name: Must be set to "KeePass Plugin" (without the quotes).
        /// </summary>
        [TestMethod]
        public void AssemblyProduct_ShouldBeEqualToKeePassPlugin()
        {
            // Arrange
            var pluginAssembly = typeof(KeeTrayTOTPExt).Assembly;

            // Act
            var assemblyProduct = pluginAssembly.GetCustomAttribute<AssemblyProductAttribute>();

            // Assert
            assemblyProduct.Should().NotBeNull();
            assemblyProduct.Product.Should().Be("KeePass Plugin");
        }

        /// <summary>
        /// The namespace must be named like the DLL file without extension.
        /// </summary>
        [TestMethod]
        public void AssemblyNamespace_ShouldBeEqualToAssemblyName()
        {
            // Arrange
            var pluginType = typeof(KeeTrayTOTPExt);
            var pluginAssembly = typeof(KeeTrayTOTPExt).Assembly;

            // Assert
            pluginType.Namespace.Should().Be(pluginAssembly.GetName().Name);
        }

        /// <summary>
        /// The plugin class must be named like the namespace plus "Ext".
        /// </summary>
        [TestMethod]
        public void PluginClass_ShouldBeEqualToNamespacePlusExt()
        {
            // Arrange
            var pluginType = typeof(KeeTrayTOTPExt);

            // Assert
            pluginType.Name.Should().Be($"{pluginType.Namespace}Ext");
        }


        /// <summary>
        /// The plugin class must derive from the Plugin class
        /// </summary>
        [TestMethod]
        public void PluginClass_ShouldDeriveFromIPlugin()
        {
            // Arrange
            var pluginType = typeof(KeeTrayTOTPExt);

            // Assert
            pluginType.Should().BeDerivedFrom<Plugin>();
        }
    }
}
