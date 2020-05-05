using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Time_Correction Collection.
    /// </summary>
    public class TimeCorrectionCollection
    {
        /// <summary>
        /// Time Correction List.
        /// </summary>
        private readonly List<TimeCorrectionProvider> _timeCorrections;
        private bool _enable;
        /// <summary>
        /// Enables or disables the Time Correction verification for all the collection items.
        /// </summary>
        internal bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                foreach (var timeCorrection in _timeCorrections)
                {
                    timeCorrection.Enable = value;
                }
            }
        }

        /// <summary>
        /// Provides access to a specific collection item using the URL as a key.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        internal TimeCorrectionProvider this[string url]
        {
            get
            {
                foreach (var timeCorrection in _timeCorrections)
                {
                    if (timeCorrection.Url == url)
                    {
                        return timeCorrection;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Handles Time Correction for TOTP Generators insuring generation accuracy.
        /// </summary>
        /// <param name="enable">Enabled by Default.</param>
        internal TimeCorrectionCollection(bool enable = true)
        {
            _enable = enable;
            _timeCorrections = new List<TimeCorrectionProvider>();
        }

        /// <summary>
        /// Populates the Time Correction Collection with the URL list specified.
        /// </summary>
        /// <param name="urLs">URLs.</param>
        internal void AddRangeFromList(IEnumerable<string> urLs)
        {
            foreach (var url in urLs)
            {
                //Validating that url is not null.
                if (url != string.Empty)
                {
                    //Validating that this server is not already checked.
                    bool next = false;
                    foreach (var timeC in _timeCorrections)
                    {
                        if (timeC.Url == url)
                        {
                            next = true;
                        }
                    }
                    //Adding server to time correction collection.
                    if (!next)
                    {
                        _timeCorrections.Add(new TimeCorrectionProvider(url, _enable));
                    }
                }
            }
        }

        /// <summary>
        /// Clears the Time Correction Collection and populates it with the string specified.
        /// </summary>
        /// <param name="urLs">URLs.</param>
        internal void ResetThenAddRangeFromString(string urLs)
        {
            _timeCorrections.Clear();
            foreach (string url in urLs.Split(';'))
            {
                //Validating that url is not null.
                if (url != string.Empty)
                {
                    //Validating that this server is not already checked.
                    bool next = false;
                    foreach (var timeC in _timeCorrections)
                    {
                        if (timeC.Url == url)
                        {
                            next = true;
                        }
                    }
                    //Adding server to time correction collection.
                    if (!next)
                    {
                        _timeCorrections.Add(new TimeCorrectionProvider(url, _enable));
                    }
                }
            }
        }

        /// <summary>
        /// Clears the Time Correction Collection and populates it with the Listviewitemcollection specified.
        /// </summary>
        /// <param name="lvIs">ListViewItem Collection containing all Time Correction Servers.</param>
        internal void ResetThenAddRangeFromLvIs(ListView.ListViewItemCollection lvIs)
        {
            _timeCorrections.Clear();
            foreach (ListViewItem lvi in lvIs)
            {
                //Validating that url is not null.
                if (lvi.Text != string.Empty)
                {
                    //Validating that this server is not already checked.
                    bool next = false;
                    foreach (var timeC in _timeCorrections)
                    {
                        if (timeC.Url == lvi.Text)
                        {
                            next = true;
                        }
                    }
                    //Adding server to time correction collection.
                    if (!next)
                    {
                        _timeCorrections.Add(new TimeCorrectionProvider(lvi.Text, _enable));
                    }
                }
            }
        }

        /// <summary>
        /// Returns all URLs with their current timespan or connection status for adding to a listview.
        /// </summary>
        /// <returns>ListViewItem Array.</returns>
        internal ListViewItem[] ToLvi()
        {
            //Temporary Listviewitem List to facilitate building the array.
            var lvIs = new List<ListViewItem>();
            foreach (var tc in _timeCorrections)
            {
                //Create new Listviewitem to appear in Time Correction Settings Listview.
                var lvi = new ListViewItem(tc.Url) { ImageIndex = tc.LastUpdateSucceeded ? 0 : 2 };
                lvi.SubItems.Add(tc.LastUpdateSucceeded ? tc.TimeCorrection.ToString() : Localization.Strings.ConnectionFailed);
                lvi.Tag = tc;
                lvi.ToolTipText = tc.LastUpdateSucceeded ? string.Empty : tc.LastUpdateDateTime.ToString();
                lvIs.Add(lvi);
            }
            return lvIs.ToArray();
        }

        /// <summary>
        /// Returns all URLs in a string array for adding range to combobox.
        /// </summary>
        /// <returns>String Array.</returns>
        internal object[] ToComboBox()
        {
            //Temporary string List to facilitate building the array.
            var @return = new List<object>();
            foreach (var timeCorrection in _timeCorrections)
            {
                @return.Add(timeCorrection.Url);
            }
            return @return.ToArray();
        }

        /// <summary>
        /// Returns all time correction URLs
        /// </summary>
        internal IEnumerable<string> GetTimeCorrectionUrls()
        {
            return _timeCorrections.Select(c => c.Url);
        }
    }
}