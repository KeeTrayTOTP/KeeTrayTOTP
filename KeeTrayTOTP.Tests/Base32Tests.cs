using FluentAssertions;
using KeeTrayTOTP.Libraries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeTrayTOTP.Tests
{
    [TestClass]
    public class Base32Tests
    {
        private const string InvalidSeed = "MZXW6!YTBOI======!";

        [DataTestMethod]
        [DynamicData(nameof(GetBase32Data), DynamicDataSourceType.Method)]
        public void Encode_ShouldWorkOnRfc4648TestCases(Base32TestCase testCase)
        {
            var actual = Base32.Encode(Encoding.UTF8.GetBytes(testCase.Decoded));

            actual.Should().Be(testCase.Encoded);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetBase32Data), DynamicDataSourceType.Method)]
        public void Decode_ShouldWorkOnRfc4648TestCases(Base32TestCase testCase)
        {
            var actual = Base32.Decode(testCase.Encoded);

            var actualString = Encoding.UTF8.GetString(actual);

            actualString.Should().Be(testCase.Decoded);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetBase32Data), DynamicDataSourceType.Method)]
        public void IsBase32Out_ShouldReturnTrueFor(Base32TestCase testCase)
        {
            var actual = testCase.Encoded.IsBase32(out string invalidChars);

            actual.Should().BeTrue();
            invalidChars.Should().BeNullOrEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetBase32Data), DynamicDataSourceType.Method)]
        public void IsBase32_ShouldReturnTrueFor(Base32TestCase testCase)
        {
            var actual = testCase.Encoded.IsBase32();

            actual.Should().BeTrue();
        }

        [TestMethod]
        public void Decode_ShouldThrowWithInvalidSeed()
        {
            Action act = () => Base32.Decode(InvalidSeed);

            act.Should().Throw<ArgumentException>().WithMessage("Base32 contains illegal characters.");
        }

        [TestMethod]
        public void ExtIsBase32_ShouldReturnFalseWithInvalidChars()
        {
            var actual = InvalidSeed.IsBase32(out string invalidChars);

            actual.Should().BeFalse();
            invalidChars.Should().Be("!======!");
        }

                /// <summary>
        /// Testcases were taken from <see cref="https://tools.ietf.org/html/rfc4648"/>
        /// </summary>
        public static IEnumerable<object[]> GetBase32Data()
        {
            yield return new object[] { new Base32TestCase("", "") };
            yield return new object[] { new Base32TestCase("f", "MY======") };
            yield return new object[] { new Base32TestCase("fo", "MZXQ====") };
            yield return new object[] { new Base32TestCase("foo", "MZXW6===") };
            yield return new object[] { new Base32TestCase("foob", "MZXW6YQ=") };
            yield return new object[] { new Base32TestCase("fooba", "MZXW6YTB") };
            yield return new object[] { new Base32TestCase("foobar", "MZXW6YTBOI======") };
        }

        public sealed class Base32TestCase
        {
            public Base32TestCase(string decoded, string encoded)
            {
                this.Decoded = decoded;
                this.Encoded = encoded;
            }

            /// <summary>
            /// Input for Encoding, expected Output for Decoding
            /// </summary>
            public string Decoded { get; }

            /// <summary>
            /// String in Base32 encoding
            /// </summary>
            public string Encoded { get; }

            public override string ToString()
            {
                return $"Decoded = {Decoded}, Encoded = {Encoded}";
            }
        }
    }
}
