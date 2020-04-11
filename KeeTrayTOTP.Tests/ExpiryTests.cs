using KeePassLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class ExpiryTests
    {
        [TestMethod]
        public void IsExpired_ShouldReturnTrue_WhenExpiresIsTrueAndExpiryTimeInThePast()
        {
            var pwEntry = new PwEntry(true, true)
            {
                Expires = true,
                ExpiryTime = DateTime.MinValue
            };

            Assert.IsTrue(pwEntry.IsExpired());
        }

        [TestMethod]
        public void IsExpired_ShouldReturnFalse_WhenExpiresIsTrueAndExpiryTimeInTheFuture()
        {
            var pwEntry = new PwEntry(true, true)
            {
                Expires = true,
                ExpiryTime = DateTime.UtcNow.AddSeconds(2)
            };

            Assert.IsFalse(pwEntry.IsExpired());
        }

        [TestMethod]
        public void IsExpired_ShouldReturnFalse_WhenExpiresIsFalseAndExpiryTimeInThePast()
        {
            var pwEntry = new PwEntry(true, true)
            {
                Expires = false,
                ExpiryTime = DateTime.MinValue
            };

            Assert.IsFalse(pwEntry.IsExpired());
        }
    }
}
