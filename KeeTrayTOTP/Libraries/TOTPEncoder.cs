using System;
using System.Text;

namespace KeeTrayTOTP.Libraries
{
    internal static class TOTPEncoder
    {
        /// <summary>
		/// Character set for authenticator code
		/// </summary>
		private static readonly char[] Steamchars = new char[] {
                '2', '3', '4', '5', '6', '7', '8', '9', 'B', 'C',
                'D', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q',
                'R', 'T', 'V', 'W', 'X', 'Y'};

        public static readonly Func<byte[], int, string> Rfc6238 = (byte[] bytes, int length) =>
        {
            var fullcode = OTP2UInt(bytes);
            var mask = (uint)Math.Pow(10, length);

            return (fullcode % mask).ToString(new string('0', length));
        };

        public static readonly Func<byte[], int, string> Steam = (byte[] bytes, int length) =>
        {
            var fullcode = OTP2UInt(bytes);

            StringBuilder code = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                code.Append(Steamchars[fullcode % Steamchars.Length]);
                fullcode /= (uint)Steamchars.Length;
            }

            return code.ToString();
        };

        private static uint OTP2UInt(byte[] totp)
        {
            return BitConverter.ToUInt32(totp, 0) & 0x7fffffff;
        }
    }
}
