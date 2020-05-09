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
        public TimeSpan TimeCorrection { get; set; }

        public bool TimeCorrectionError { get; private set; }

        /// <summary>
        /// Instantiate a new TOTP_Generator.
        /// </summary>
        public TOTPProvider(string[] settings, TimeCorrectionCollection tcc)
        {
            this._duration = Convert.ToInt16(settings[0]);

            if (settings[1] == "S")
            {
                this._length = 5;
                this.Encoder = TOTPEncoder.Steam;
            }
            else
            {
                this._length = Convert.ToInt16(settings[1]);
                this.Encoder = TOTPEncoder.Rfc6238;
            }

            if (settings.Length > 2 && settings[2] != string.Empty)
            {
                var tc = tcc[settings[2]];

                if (tc != null)
                {
                    this.TimeCorrection = tc.TimeCorrection;
                }
                else
                {
                    this.TimeCorrection = TimeSpan.Zero;
                    this.TimeCorrectionError = false;
                }
            }
            else
            {
                this.TimeCorrection = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Returns current time with correction int UTC format.
        /// </summary>
        public DateTime Now
        {
            get
            {
                return DateTime.UtcNow - TimeCorrection; // Computes current time minus time correction giving the corrected time.
            }
        }

        /// <summary>
        /// Returns the time remaining before counter incrementation.
        /// </summary>
        public int Timer
        {
            get
            {
                var n = _duration - (int)((Now - UnixEpoch).TotalSeconds % _duration); // Computes the seconds left before counter incrementation.
                return n == 0 ? _duration : n; // Returns timer value from 30 to 1.
            }
        }

        /// <summary>
        /// Returns number of intervals that have elapsed.
        /// </summary>
        public long Counter
        {
            get
            {
                var elapsedSeconds = (long)Math.Floor((Now - UnixEpoch).TotalSeconds); //Compute current counter for current time.
                return elapsedSeconds / _duration; //Applies specified interval to computed counter.
            }
        }

        /// <summary>
        /// Generate a TOTP using provided binary data.
        /// </summary>
        /// <param name="key">Key in String Format.</param>
        /// <returns>Time-based One Time Password encoded byte array.</returns>
        public string Generate(string key)
        {
            return this.GenerateByByte(Base32.Decode(key));
        }

        /// <summary>
        /// Generate a TOTP using provided binary data.
        /// </summary>
        /// <param name="key">Binary data.</param>
        /// <returns>Time-based One Time Password encoded byte array.</returns>
        public string GenerateByByte(byte[] key)
        {
            byte[] codeInterval = BitConverter.GetBytes((ulong)Counter);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(codeInterval);
            }

            using (var hmac = new HMACSHA1(key, true))
            {
                byte[] hash = hmac.ComputeHash(codeInterval); //Generates hash from key using counter.
                hmac.Clear(); //Clear hash instance securing the key.

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