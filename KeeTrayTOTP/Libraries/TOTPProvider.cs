using KeePassLib;
using KeeTrayTOTP.Helpers;
using System;
using System.Security.Cryptography;

namespace KeeTrayTOTP.Libraries
{
    /// <summary>
    /// Provides Time-based One Time Passwords RFC 6238.
    /// </summary>
    public class TOTPProvider
    {
        /// <summary>
        /// Time reference for TOTP generation.
        /// </summary>
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly byte[] _seed;

        /// <summary>
        /// Duration of generation of each totp, in seconds.
        /// </summary>
        private readonly int _duration;

        /// <summary>
        /// Length of the generated totp.
        /// </summary>
        private readonly int _length;

        public Func<byte[], int, string> Encoder { get; set; }

        /// <summary>
        /// Sets the time span that is used to match the server's UTC time to ensure accurate generation of Time-based One Time Passwords.
        /// </summary>
        private TimeSpan _timeCorrection;

        public TOTPProvider(TOTPEntryValidator totpEntryValidator, PwEntry entry, TimeCorrectionCollection tcc)
        {
            var keyUri = totpEntryValidator.ReadAsKeyUri(entry);

            this._seed = Base32.Decode(keyUri.Secret);
            this._duration = keyUri.Period;
            this._length = keyUri.Digits;
            this._timeCorrection = tcc.GetTimeCorrection(keyUri.TimeCorrectionUrl);
            this.Encoder = keyUri.Digits == 5 ? TOTPEncoder.Steam : TOTPEncoder.Rfc6238;
        }

        /// <summary>
        /// Returns the time remaining before counter incrementation.
        /// </summary>
        public int SecondsRemaining
        {
            get
            {
                var n = _duration - (int)((TimeCorrectedUtcNow - UnixEpoch).TotalSeconds % _duration); // Computes the seconds left before counter incrementation.
                return n == 0 ? _duration : n; // Returns timer value from 30 to 1.
            }
        }

        /// <summary>
        /// Returns current time with correction in UTC format.
        /// </summary>
        private DateTime TimeCorrectedUtcNow
        {
            get
            {
                return DateTime.UtcNow - _timeCorrection; // Computes current time minus time correction giving the corrected time.
            }
        }

        /// <summary>
        /// Returns number of intervals that have elapsed.
        /// </summary>
        private ulong Counter
        {
            get
            {
                var elapsedSeconds = (long)Math.Floor((TimeCorrectedUtcNow - UnixEpoch).TotalSeconds); // Compute current counter for current time.
                return (ulong)(elapsedSeconds / _duration); // Applies specified interval to computed counter.
            }
        }

        /// <summary>
        /// Generate a TOTP using provided binary data.
        /// </summary>
        /// <returns>Time-based One Time Password encoded byte array.</returns>
        public string Generate()
        {
            var codeInterval = BitConverter.GetBytes(Counter);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(codeInterval);
            }

            using (var hmac = new HMACSHA1(_seed, true))
            {
                byte[] hash = hmac.ComputeHash(codeInterval);
                hmac.Clear();

                int start = hash[hash.Length - 1] & 0xf;
                byte[] totp = new byte[4];

                Array.Copy(hash, start, totp, 0, 4);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(totp);
                }

                return this.Encoder(totp, _length);
            }
        }
    }
}