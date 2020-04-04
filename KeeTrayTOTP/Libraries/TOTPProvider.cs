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
        private int _duration;
        public int Duration
        {
            get
            {
                return this._duration;
            }
            set
            {
                if (!(value > 0)) throw new Exception("Invalid Duration."); //Throws an exception if the duration is invalid as the class cannot work without it.
                this._duration = value; //Defines variable from argument.
            }
        }

        /// <summary>
        /// Length of the generated totp.
        /// </summary>
        private int _length;
        public int Length
        {
            get
            {
                return this._length;
            }
            set
            {
                //Throws an exception if the length is invalid as the class cannot work without it.
                if (value < 4 || value > 8) throw new Exception("Invalid Length.");
                this._length = value; //Defines variable from argument.
            }

        }


        /// <summary>
        /// TOTP Encoder.
        /// </summary>
        private Func<byte[], int, string> _encoder;
        public Func<byte[], int, string> Encoder
        {
            get
            {
                return this._encoder;
            }
            set
            {
                this._encoder = value; //Defines variable from argument.
            }
        }

        /// <summary>
        /// Sets the time span that is used to match the server's UTC time to ensure accurate generation of Time-based One Time Passwords.
        /// </summary>
        private TimeSpan _timeCorrection;
        public TimeSpan TimeCorrection
        {
            get
            {
                return this._timeCorrection;
            }
            set
            {
                this._timeCorrection = value; //Defines variable from argument.
            }
        }

        private bool _timeCorrectionError;
        public bool TimeCorrectionError
        {
            get
            {
                return this._timeCorrectionError;
            } 
        }

        /// <summary>
        /// Instanciates a new TOTP_Generator.
        /// </summary>
        /// <param name="initDuration">Duration of generation of each totp, in seconds.</param>
        /// <param name="initLength">Length of the generated totp.</param>
        /// <param name="initEncoder">The output encoder.</param>
        /*public TOTPProvider(int initDuration, int initLength, Func<byte[], int, string> initEncoder)
        {
            this.Duration = initDuration;
            this.Length = initLength;
            this.encoder = initEncoder;
            this.TimeCorrection = TimeSpan.Zero;
        }*/

        /// <summary>
        /// Instanciates a new TOTP_Generator.
        /// </summary>
        /// <param name="initSettings">Saved Settings.</param>
        public TOTPProvider(string[] settings, ref TimeCorrectionCollection tcc)
        {
            this._duration = Convert.ToInt16(settings[0]);

            if (settings[1] == "S")
            {
                this._length = 5;
                this._encoder = TOTPEncoder.Steam;
            }
            else
            {
                this._length = Convert.ToInt16(settings[1]);
                this._encoder = TOTPEncoder.Rfc6238;
            }

            if(settings.Length > 2 && settings[2] != String.Empty)
            {
                var tc = tcc[settings[2]];

                if (tc != null)
                    this.TimeCorrection = tc.TimeCorrection;
                else
                {
                    this.TimeCorrection = TimeSpan.Zero;
                    this._timeCorrectionError = false;
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
                return DateTime.UtcNow - _timeCorrection; //Computes current time minus time correction giving the corrected time.
            }
        }

        /// <summary>
        /// Returns the time remaining before counter incrementation.
        /// </summary>
        public int Timer
        {
            get
            {
                var n = (_duration - (int)((Now - UnixEpoch).TotalSeconds % _duration)); //Computes the seconds left before counter incrementation.
                return n == 0 ? _duration : n; //Returns timer value from 30 to 1.
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
        /// Converts an unsigned integer to binary data.
        /// </summary>
        /// <param name="n">Unsigned Integer.</param>
        /// <returns>Binary data.</returns>
        private byte[] GetBytes(ulong n)
        {
            byte[] b = new byte[8]; //Math.
            b[0] = (byte)(n >> 56); //Math.
            b[1] = (byte)(n >> 48); //Math.
            b[2] = (byte)(n >> 40); //Math.
            b[3] = (byte)(n >> 32); //Math.
            b[4] = (byte)(n >> 24); //Math.
            b[5] = (byte)(n >> 16); //Math.
            b[6] = (byte)(n >> 8);  //Math.
            b[7] = (byte)(n);       //Math.
            return b;
        }

        /// <summary>
        /// Generate a TOTP using provided binary data.
        /// </summary>
        /// <param name="key">Binary data.</param>
        /// <returns>Time-based One Time Password encoded byte array.</returns>
        public byte[] Generate(byte[] key)
        {
            System.Security.Cryptography.HMACSHA1 hmac = new System.Security.Cryptography.HMACSHA1(key, true); //Instanciates a new hash provider with a key.
            byte[] hash = hmac.ComputeHash(GetBytes((ulong)Counter)); //Generates hash from key using counter.
            hmac.Clear(); //Clear hash instance securing the key.

            /*int binary =                                        //Math.
               ((hash[offset] & 0x7f) << 24)                   //Math.
               | ((hash[offset + 1] & 0xff) << 16)             //Math.
               | ((hash[offset + 2] & 0xff) << 8)              //Math.
               | (hash[offset + 3] & 0xff);                    //Math.

          int password = binary % (int)Math.Pow(10, length); //Math.*/

            int offset = hash[hash.Length - 1] & 0x0f;           //Math.
            byte[] totp = { hash[offset + 3], hash[offset + 2], hash[offset + 1], hash[offset] };
            return totp;

            /* 
             return password.ToString(new string('0', length)); //Math.*/
        }

        /// <summary>
        /// Generate a TOTP using provided binary data.
        /// </summary>
        /// <param name="key">Key in String Format.</param>
        /// <returns>Time-based One Time Password encoded byte array.</returns>
        public string Generate(string key)
        {
            byte[] bkey = Base32.Decode(key);
            return this.GenerateByByte(bkey);
        }

         /// <summary>
        /// Generate a TOTP using provided binary data.
        /// </summary>
        /// <param name="key">Binary data.</param>
        /// <returns>Time-based One Time Password encoded byte array.</returns>
        public string GenerateByByte(byte[] key)
        {

            HMACSHA1 hmac = new HMACSHA1(key, true); //Instanciates a new hash provider with a key.

            byte[] codeInterval = BitConverter.GetBytes((ulong)Counter);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(codeInterval);

            byte[] hash = hmac.ComputeHash(codeInterval); //Generates hash from key using counter.
            hmac.Clear(); //Clear hash instance securing the key.
            int start = hash[hash.Length - 1] & 0xf;
            byte[] totp = new byte[4];

            Array.Copy(hash, start, totp, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(totp);

            return this._encoder(totp, _length);
        }

    }
}