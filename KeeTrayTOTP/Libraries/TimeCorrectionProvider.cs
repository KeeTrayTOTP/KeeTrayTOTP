using System;
using System.Linq;
using System.Net;

namespace KeeTrayTOTP.Libraries
{
    /// <summary>
    /// Provides time correction for Time-based One Time Passwords that require accurate DateTime syncronisation with server.
    /// </summary>
    public class TimeCorrectionProvider
    {
        /// <summary>
        /// Timer providing the delay between each time correction check.
        /// </summary>
        private readonly System.Timers.Timer _timer;

        /// <summary>
        /// Thread which handles the time correction check.
        /// </summary>
        private System.Threading.Thread _task;

        private bool _enable;

        /// <summary>
        /// Defines whether or not the class will attempt to get time correction from the server.
        /// </summary>
        public bool Enable { get { return _enable; } set { _enable = value; _timer.Enabled = value; } }

        /// <summary>
        /// Gets or sets the interval in minutes between each online checks for time correction.
        /// </summary>
        /// <value>Time</value>
        public static int Interval { get; set; }
        private long _intervalStretcher;

        /// <summary>
        /// Returns the URL this instance is using to checks for time correction.
        /// </summary>
        public string Url { get; private set; }

        private TimeSpan _timeCorrection;
        /// <summary>
        /// Returns the time span between server UTC time and this computer's UTC time of the last check for time correction.
        /// </summary>
        public TimeSpan TimeCorrection { get { return _timeCorrection; } }

        /// <summary>
        /// Returns the date and time in universal format of the last online check for time correction.
        /// </summary>
        public DateTime LastUpdateDateTime { get; private set; }

        /// <summary>
        /// Returns true if the last check for time correction was successful.
        /// </summary>
        public bool LastUpdateSucceeded { get; private set; }

        public TimeCorrectionProvider(string url, bool enable = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Invalid URL.", "url");
            }

            Url = url;
            _enable = enable;
            LastUpdateDateTime = DateTime.MinValue;
            _timeCorrection = TimeSpan.Zero;
            _timer = new System.Timers.Timer();
            _timer.Elapsed += Timer_Elapsed;
            _timer.Interval = 1000;
            _timer.Enabled = _enable;
            _task = new System.Threading.Thread(UpdateTimeCorrection);
            if (_enable)
            {
                _task.Start();
            }
        }

        /// <summary>
        /// Task that occurs every time the timer's interval has elapsed.
        /// </summary>
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            _intervalStretcher++;
            if (_intervalStretcher >= (60 * Interval))
            {
                _intervalStretcher = 0;
                EnsureTaskIsAlive();
            }
        }

        private void EnsureTaskIsAlive()
        {
            if (!_task.IsAlive)
            {
                _task = new System.Threading.Thread(UpdateTimeCorrection);
                _task.Start();
            }
        }

        /// <summary>
        /// Attempts to get time correction from the server.
        /// </summary>
        private void UpdateTimeCorrection()
        {
            try
            {
                EnsureTls11Tls12();

                using (var webClient = new WebClient())
                {
                    webClient.DownloadData(Url);
                    var dateHeader = webClient.ResponseHeaders.Get("Date");
                    _timeCorrection = DateTime.UtcNow - DateTime.Parse(dateHeader, System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat).ToUniversalTime();
                    LastUpdateSucceeded = true;
                }
            }
            catch (Exception)
            {
                LastUpdateSucceeded = false;
            }
            LastUpdateDateTime = DateTime.Now;
        }

        /// <summary>
        /// The flags Tls11 and Tls12 in SecurityProtocolType have been
        /// introduced in .NET 4.5 and must not be set when running under
        /// older .NET versions (otherwise an exception is thrown)
        /// </summary>
        private static void EnsureTls11Tls12()
        {
            var enumType = typeof(SecurityProtocolType);
            var protocolsToEnable = Enum.GetNames(enumType)
                .Where(protocol => new[] { "Tls11", "Tls12" }.Contains(protocol, StringComparer.OrdinalIgnoreCase))
                .Select(protocol => (SecurityProtocolType)Enum.Parse(enumType, protocol, true));

            foreach (var protocolToEnable in protocolsToEnable)
            {
                ServicePointManager.SecurityProtocol |= protocolToEnable;
            }
        }
    }
}
