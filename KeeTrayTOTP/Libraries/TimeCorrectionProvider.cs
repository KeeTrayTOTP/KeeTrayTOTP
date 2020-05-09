using System;

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

        /// <summary>
        /// Instanciates a new TOTP_TimeCorrection using the specified URL to contact the server.
        /// </summary>
        /// <param name="url">URL of the server to get check.</param>
        /// <param name="enable">Enable or disable the time correction check.</param>
        public TimeCorrectionProvider(string url, bool enable = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Invalid URL.", "url");
            }

            Url = url; //Defines variable from argument.
            _enable = enable; //Defines variable from argument.
            LastUpdateDateTime = DateTime.MinValue; //Defines variable from non-constant default value.
            _timeCorrection = TimeSpan.Zero; //Defines variable from non-constant default value.
            _timer = new System.Timers.Timer(); //Instanciates timer.
            _timer.Elapsed += Timer_Elapsed; //Handles the timer event
            _timer.Interval = 1000; //Defines the timer interval to 1 seconds.
            _timer.Enabled = _enable; //Defines the timer to run if the class is initially enabled.
            _task = new System.Threading.Thread(Task_Thread); //Instanciate a new task.
            if (_enable)
            {
                _task.Start(); //Starts the new thread if the class is initially enabled.
            }
        }

        /// <summary>
        /// Task that occurs every time the timer's interval has elapsed.
        /// </summary>
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            _intervalStretcher++; //Increments timer.
            if (_intervalStretcher >= (60 * Interval)) //Checks if the specified delay has been reached.
            {
                _intervalStretcher = 0; //Resets the timer.
                Task_Do(); //Attempts to run a new task
            }
        }

        /// <summary>
        /// Instanciates a new task and starts it.
        /// </summary>
        /// <returns>Informs if reinstanciation of the task has succeeded or not. Will fail if the thread is still active from a previous time correction check.</returns>
        private bool Task_Do()
        {
            if (!_task.IsAlive) //Checks if the task is still running.
            {
                _task = new System.Threading.Thread(Task_Thread); //Instanciate a new task.
                _task.Start(); //Starts the new thread.
                return true; //Informs if successful
            }
            return false; //Informs if failed
        }

        /// <summary>
        /// Event that occurs when the timer has reached the required value. Attempts to get time correction from the server.
        /// </summary>
        private void Task_Thread()
        {
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.DownloadData(Url); //Downloads the server's page using HTTP or HTTPS.
                    var dateHeader = webClient.ResponseHeaders.Get("Date"); //Gets the date from the HTTP header of the downloaded page.
                    _timeCorrection = DateTime.UtcNow - DateTime.Parse(dateHeader, System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat).ToUniversalTime(); //Compares the downloaded date to the systems date giving us a timespan.
                    LastUpdateSucceeded = true; //Informs that the date check has succeeded.
                }
            }
            catch (Exception)
            {
                LastUpdateSucceeded = false; //Informs that the date check has failed.
            }
            LastUpdateDateTime = DateTime.Now; //Informs when the last update has been attempted (succeeded or not).
        }

        /// <summary>
        /// Perform a time correction check, may a few seconds.
        /// </summary>
        /// <param name="resetTimer">Resets the timer to 0. Occurs even if the attempt to attempt a new time correction fails.</param>
        /// <param name="forceCheck">Attempts to get time correction even if disabled.</param>
        /// <returns>Informs if the time correction check was attempted or not. Will fail if the thread is still active from a previous time correction check.</returns>
        public bool CheckNow(bool resetTimer = true, bool forceCheck = false)
        {
            if (resetTimer) //Checks if the timer should be reset.
            {
                _intervalStretcher = 0; //Resets the timer.
            }
            if (forceCheck || _enable) //Checks if this check is forced or if time correction is enabled.
            {
                return Task_Do(); //Attempts to run a new task and informs if attempt to attemp is a success of fail
            }
            return false; //Informs if not attempted to attempt
        }
    }
}
