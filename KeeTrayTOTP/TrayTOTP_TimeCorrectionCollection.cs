﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Time_Correction Collection.
    /// </summary>
    public class TimeCorrectionCollection : IEnumerable<TimeCorrectionProvider>
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
        /// <param name="Plugin">Handle to the plugin's class.</param>
        /// <param name="enable">Enabled by Default.</param>
        internal TimeCorrectionCollection(bool enable = true)
        {
            _enable = enable;
            _timeCorrections = new List<TimeCorrectionProvider>();
        }

        /// <summary>
        /// Populates the Time Correction Collection with the URLs in the specified string.
        /// </summary>
        /// <param name="urLs">URLs.</param>
        internal void AddRangeFromString(string urLs)
        {
            foreach (var url in urLs.Split(';'))
            {
                //Validating that url is not null.
                if (url != String.Empty)
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
                    if (!next) _timeCorrections.Add(new TimeCorrectionProvider(url, _enable));
                }
            }
        }

        /// <summary>
        /// Populates the Time Correction Collection with the URL list specified.
        /// </summary>
        /// <param name="urLs">URLs.</param>
        internal void AddRangeFromList(List<string> urLs)
        {
            foreach (var url in urLs)
            {
                //Validating that url is not null.
                if (url != String.Empty)
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
                    if (!next) _timeCorrections.Add(new TimeCorrectionProvider(url, _enable));
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
                if (url != String.Empty)
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
                    if (!next) _timeCorrections.Add(new TimeCorrectionProvider(url, _enable));
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
                if (lvi.Text != String.Empty)
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
                    if (!next) _timeCorrections.Add(new TimeCorrectionProvider(lvi.Text, _enable));
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
                var lvi = new ListViewItem(tc.Url) { ImageIndex = tc.LastUpdateSucceded ? 0 : 2 };
                lvi.SubItems.Add((tc.LastUpdateSucceded ? tc.TimeCorrection.ToString() : Localization.Strings.ConnectionFailed));
                lvi.Tag = tc;
                lvi.ToolTipText = (tc.LastUpdateSucceded ? String.Empty : tc.LastUpdateDateTime.ToString());
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
        /// Returns all URLs in one string in order to save them to KeePass settings.
        /// </summary>
        /// <returns>String seperated by a colon.</returns>
        internal string ToSetting()
        {
            //Temporary string to build the string from multiple strings.
            var @return = String.Empty;
            foreach (var timeCorrection in _timeCorrections)
            {
                @return = @return + timeCorrection.Url + ";";
            }
            return @return.TrimEnd(';');
        }

        /// <summary>
        /// Support the enumeration of the collection to handle [foreach (var VARIABLE in...].
        /// </summary>
        /// <returns>Time Correction List Enumerator.</returns>
        public IEnumerator<TimeCorrectionProvider> GetEnumerator()
        {
            return _timeCorrections.GetEnumerator();
        }

        /// <summary>
        /// Support the enumeration of the collection to handle [foreach (TimeCorrection_Provider VARIABLE in...].
        /// </summary>
        /// <returns>Time Correction List Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _timeCorrections.GetEnumerator();
        }
    }
}