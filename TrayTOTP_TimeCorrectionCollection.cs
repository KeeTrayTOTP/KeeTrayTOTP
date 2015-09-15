using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Time_Correction Collection.
    /// </summary>
    internal class TimeCorrection_Collection : IEnumerable<TimeCorrectionProvider>
    {
        /// <summary>
        /// Reference to the main plugin class.
        /// </summary>
        private readonly KeeTrayTOTPExt plugin;
        /// <summary>
        /// Time Correction List.
        /// </summary>
        private readonly List<TimeCorrectionProvider> TimeCorrections;
        private bool _Enable;
        /// <summary>
        /// Enables or disables the Time Correction verification for all the collection items.
        /// </summary>
        internal bool Enable
        {
            get { return _Enable; }
            set
            {
                _Enable = value;
                foreach (var TimeCorrection in TimeCorrections)
                {
                    TimeCorrection.Enable = value;
                }
            }
        }

        /// <summary>
        /// Provides access to a specific collection item using the URL as a key.
        /// </summary>
        /// <param name="URL">URL.</param>
        /// <returns></returns>
        internal TimeCorrectionProvider this[string URL]
        {
            get
            {
                foreach (var TimeCorrection in TimeCorrections)
                {
                    if (TimeCorrection.Url == URL)
                    {
                        return TimeCorrection;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Handles Time Correction for TOTP Generators insuring generation accuracy.
        /// </summary>
        /// <param name="Plugin">Handle to the plugin's class.</param>
        /// <param name="Enable">Enabled by Default.</param>
        internal TimeCorrection_Collection(KeeTrayTOTPExt Plugin, bool Enable = true)
        {
            plugin = Plugin;
            _Enable = Enable;
            TimeCorrections = new List<TimeCorrectionProvider>();
        }

        /// <summary>
        /// Populates the Time Correction Collection with the URLs in the specified string.
        /// </summary>
        /// <param name="URLs">URLs.</param>
        internal void AddRangeFromString(string URLs)
        {
            foreach (var URL in URLs.Split(';'))
            {
                //Validating that url is not null.
                if (URL != String.Empty)
                {
                    //Validating that this server is not already checked.
                    bool next = false;
                    foreach (var TimeC in TimeCorrections)
                    {
                        if (TimeC.Url == URL)
                        {
                            next = true;
                        }
                    }
                    //Adding server to time correction collection.
                    if (!next) TimeCorrections.Add(new TimeCorrectionProvider(URL, _Enable));
                }
            }
        }

        /// <summary>
        /// Populates the Time Correction Collection with the URL list specified.
        /// </summary>
        /// <param name="URLs">URLs.</param>
        internal void AddRangeFromList(List<string> URLs)
        {
            foreach (var URL in URLs)
            {
                //Validating that url is not null.
                if (URL != String.Empty)
                {
                    //Validating that this server is not already checked.
                    bool next = false;
                    foreach (var TimeC in TimeCorrections)
                    {
                        if (TimeC.Url == URL)
                        {
                            next = true;
                        }
                    }
                    //Adding server to time correction collection.
                    if (!next) TimeCorrections.Add(new TimeCorrectionProvider(URL, _Enable));
                }
            }
        }

        /// <summary>
        /// Clears the Time Correction Collection and populates it with the string specified.
        /// </summary>
        /// <param name="URLs">URLs.</param>
        internal void ResetThenAddRangeFromString(string URLs)
        {
            TimeCorrections.Clear();
            foreach (string URL in URLs.Split(';'))
            {
                //Validating that url is not null.
                if (URL != String.Empty)
                {
                    //Validating that this server is not already checked.
                    bool next = false;
                    foreach (var TimeC in TimeCorrections)
                    {
                        if (TimeC.Url == URL)
                        {
                            next = true;
                        }
                    }
                    //Adding server to time correction collection.
                    if (!next) TimeCorrections.Add(new TimeCorrectionProvider(URL, _Enable));
                }
            }
        }

        /// <summary>
        /// Clears the Time Correction Collection and populates it with the Listviewitemcollection specified.
        /// </summary>
        /// <param name="LVIs">ListViewItem Collection containing all Time Correction Servers.</param>
        internal void ResetThenAddRangeFromLVIs(ListView.ListViewItemCollection LVIs)
        {
            TimeCorrections.Clear();
            foreach (ListViewItem LVI in LVIs)
            {
                //Validating that url is not null.
                if (LVI.Text != String.Empty)
                {
                    //Validating that this server is not already checked.
                    bool next = false;
                    foreach (var TimeC in TimeCorrections)
                    {
                        if (TimeC.Url == LVI.Text)
                        {
                            next = true;
                        }
                    }
                    //Adding server to time correction collection.
                    if (!next) TimeCorrections.Add(new TimeCorrectionProvider(LVI.Text, _Enable));
                }
            }
        }

        /// <summary>
        /// Returns all URLs with their current timespan or connection status for adding to a listview.
        /// </summary>
        /// <returns>ListViewItem Array.</returns>
        internal ListViewItem[] ToLVI()
        {
            //Temporary Listviewitem List to facilitate building the array.
            var LVIs = new List<ListViewItem>();
            foreach (var TC in TimeCorrections)
            {
                //Create new Listviewitem to appear in Time Correction Settings Listview.
                var LVI = new ListViewItem(TC.Url) { ImageIndex = TC.LastUpdateSucceded ? 0 : 2 };
                LVI.SubItems.Add((TC.LastUpdateSucceded ? TC.TimeCorrection.ToString() : TrayTOTP_TimeCorrectionCollection_Localization.ConnectionFailed));
                LVI.Tag = TC;
                LVI.ToolTipText = (TC.LastUpdateSucceded ? String.Empty : TC.LastUpdateDateTime.ToString());
                LVIs.Add(LVI);
            }
            return LVIs.ToArray();
        }

        /// <summary>
        /// Returns all URLs in a string array for adding range to combobox.
        /// </summary>
        /// <returns>String Array.</returns>
        internal object[] ToComboBox()
        {
            //Temporary string List to facilitate building the array.
            var Return = new List<object>();
            foreach (var TimeCorrection in TimeCorrections)
            {
                Return.Add(TimeCorrection.Url);
            }
            return Return.ToArray();
        }

        /// <summary>
        /// Returns all URLs in one string in order to save them to KeePass settings.
        /// </summary>
        /// <returns>String seperated by a colon.</returns>
        internal string ToSetting()
        {
            //Temporary string to build the string from multiple strings.
            var Return = String.Empty;
            foreach (var TimeCorrection in TimeCorrections)
            {
                Return = Return + TimeCorrection.Url + ";";
            }
            return Return.TrimEnd(';');
        }

        /// <summary>
        /// Support the enumeration of the collection to handle [foreach (var VARIABLE in...].
        /// </summary>
        /// <returns>Time Correction List Enumerator.</returns>
        public IEnumerator<TimeCorrectionProvider> GetEnumerator()
        {
            return TimeCorrections.GetEnumerator();
        }

        /// <summary>
        /// Support the enumeration of the collection to handle [foreach (TimeCorrection_Provider VARIABLE in...].
        /// </summary>
        /// <returns>Time Correction List Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return TimeCorrections.GetEnumerator();
        }
    }
}