// Taken from https://raw.githubusercontent.com/telehash/telehash.net/master/Telehash.Net/Base32.cs
//
// Copyright(c) 2015 Thomas Muldowney
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

namespace KeeTrayTOTP.Libraries
{
    /// <summary>
    /// Utility to deal with Base32 encoding and decoding
    /// </summary>
    /// <remarks>
    /// http://tools.ietf.org/html/rfc4648
    /// </remarks>
    public static class Base32
    {
        /// <summary>
        /// The number of bits in a base32 encoded character
        /// </summary>
        private const int EncodedBitCount = 5;
        /// <summary>
        /// The number of bits in a byte
        /// </summary>
        private const int ByteBitCount = 8;
        /// <summary>
        /// A string containing all of the base32 characters in order.
        /// This allows a simple indexof or [index] to convert between
        /// a numeric value and an encoded character and vice versa.
        /// </summary>
        private const string EncodingChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        /// <summary>
        /// The rfc defines '=' as the padding character
        /// </summary>
        private const char PaddingCharacter = '=';
        /// <summary>
        /// Takes a block of data and converts it to a base 32 encoded string
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>base 32 string</returns>
        public static string Encode(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data.Length == 0)
            {
                return string.Empty;
            }

            // The output character count is calculated in 40 bit blocks.  That is because the least
            // common blocks size for both binary (8 bit) and base 32 (5 bit) is 40.  Padding must be used
            // to fill in the difference.
            var outputCharacterCount = (int)decimal.Ceiling(data.Length / (decimal)EncodedBitCount) * ByteBitCount;
            char[] outputBuffer = new char[outputCharacterCount];

            byte workingValue = 0;
            short remainingBits = EncodedBitCount;
            int currentPosition = 0;

            foreach (byte workingByte in data)
            {
                workingValue = (byte)(workingValue | (workingByte >> (ByteBitCount - remainingBits)));
                outputBuffer[currentPosition++] = EncodingChars[workingValue];

                if (remainingBits <= ByteBitCount - EncodedBitCount)
                {
                    workingValue = (byte)((workingByte >> (ByteBitCount - EncodedBitCount - remainingBits)) & 31);
                    outputBuffer[currentPosition++] = EncodingChars[workingValue];
                    remainingBits += EncodedBitCount;
                }

                remainingBits -= ByteBitCount - EncodedBitCount;
                workingValue = (byte)((workingByte << remainingBits) & 31);
            }

            // If we didn't finish, write the last current working char
            if (currentPosition != outputCharacterCount)
            {
                outputBuffer[currentPosition++] = EncodingChars[workingValue];
            }

            // RFC 4648 specifies that padding up to the end of the next 40 bit block must be provided
            // Since the outputCharacterCount does account for the paddingCharacters, fill it out.
            while (currentPosition < outputCharacterCount)
            {
                outputBuffer[currentPosition++] = PaddingCharacter;
            }

            return new string(outputBuffer);
        }

        /// <summary>
        /// Takes a base 32 encoded value and converts it back to binary data.
        /// </summary>
        /// <param name="base32">Base 32 encoded string.</param>
        /// <returns>Binary data.</returns>
        public static byte[] Decode(string base32)
        {
            if (string.IsNullOrEmpty(base32))
            {
                return new byte[0];
            }

            var unpaddedBase32 = GetUnpaddedBase32(base32);

            foreach (var c in unpaddedBase32)
            {
                if (EncodingChars.IndexOf(c) < 0)
                {
                    throw new ArgumentException("Base32 contains illegal characters.");
                }
            }

            // we have already removed the padding so this will tell us how many actual bytes there should be.
            int outputByteCount = unpaddedBase32.Length * EncodedBitCount / ByteBitCount;
            byte[] outputBuffer = new byte[outputByteCount];

            byte workingByte = 0;
            short bitsRemaining = ByteBitCount;
            int mask = 0;
            int arrayIndex = 0;

            foreach (char workingChar in unpaddedBase32)
            {
                int encodedCharacterNumericValue = EncodingChars.IndexOf(workingChar);

                if (bitsRemaining > EncodedBitCount)
                {
                    mask = encodedCharacterNumericValue << (bitsRemaining - EncodedBitCount);
                    workingByte = (byte)(workingByte | mask);
                    bitsRemaining -= EncodedBitCount;
                }
                else
                {
                    mask = encodedCharacterNumericValue >> (EncodedBitCount - bitsRemaining);
                    workingByte = (byte)(workingByte | mask);
                    outputBuffer[arrayIndex++] = workingByte;
                    workingByte = (byte)(encodedCharacterNumericValue << (ByteBitCount - EncodedBitCount + bitsRemaining));
                    bitsRemaining += ByteBitCount - EncodedBitCount;
                }
            }

            return outputBuffer;
        }

        public static bool HasInvalidPadding(string input)
        {
            return GetUnpaddedBase32(input).Contains(PaddingCharacter);
        }

        public static bool IsBase32(this string input)
        {
            return GetUnpaddedBase32(input).All(EncodingChars.Contains);
        }

        public static bool IsBase32(this string input, out string invalidCharacters)
        {
            var hashSet = new HashSet<char>();
            foreach (var c in GetUnpaddedBase32(input))
            {
                if (!EncodingChars.Contains(c))
                {
                    hashSet.Add(c);
                }
            }

            invalidCharacters = new string(hashSet.ToArray());

            return hashSet.Count == 0;
        }

        private static string GetUnpaddedBase32(string input)
        {
            return input.ToUpperInvariant().TrimEnd(PaddingCharacter);
        }
    }
}
