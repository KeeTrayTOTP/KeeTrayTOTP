using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using KeePass.Plugins;
using KeePass.UI;

using KeePass.App.Configuration;
using KeePassLib;
using System.Collections.Concurrent;
using KeeTrayTOTP.Libraries;
using System;

namespace KeeTrayTOTP
{
    internal class KeePassUIReference
    {
        private readonly ConcurrentDictionary<PwEntry, TOTPProvider> _cache = new ConcurrentDictionary<PwEntry, TOTPProvider>();
        private readonly ConcurrentDictionary<PwEntry, ListViewItem> _pwToLvicache = new ConcurrentDictionary<PwEntry, ListViewItem>();
        private readonly ConcurrentDictionary<PwEntry, DateTime> _pwLastModifiedcache = new ConcurrentDictionary<PwEntry, DateTime>();

        private uint _lastCount;
        private PwGroup _liGroupsPreviousSelected;
        private readonly KeeTrayTOTPExt _plugin;
        private readonly CustomListViewEx _listView;
        private readonly IPluginHost _pluginHost;

        public KeePassUIReference(IPluginHost pluginHost, KeeTrayTOTPExt plugin)
        {
            _pluginHost = pluginHost;
            _plugin = plugin;
            _listView = GetAllControls(pluginHost.MainWindow).OfType<CustomListViewEx>().FirstOrDefault(c => c.Name == "m_lvEntries");
        }

        /// <summary>
        /// Get all child controls
        /// </summary>
        /// <param name="control"></param>
        private IEnumerable<Control> GetAllControls(Control control)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(GetAllControls).Concat(controls);
        }

        public void UpdateTotpColumns()
        {
            if (_listView == null)
            {
                // If we got here something in the Keepass UI listview changed.
                return;
            }

            var totpIndex = GetColumnIndex();
            if (totpIndex == -1) { return; }

            ClearCacheIfNecessary();

            if (_pwToLvicache.IsEmpty)
            {
                BuildPwEntryToListViewItemCache();
            }

            try
            {
                _listView.BeginUpdate();

                foreach (var kv in _pwToLvicache)
                {
                    kv.Value.SubItems[totpIndex].Text = GenerateOtpColumnValue(kv.Key);
                }
            }
            finally
            {
                _listView.EndUpdate();
            }
        }

        private void ClearCacheIfNecessary()
        {
            var selectedGroup = _pluginHost.MainWindow.GetSelectedGroup();
            var count = selectedGroup.GetEntriesCount(true);

            if (selectedGroup != _liGroupsPreviousSelected || _lastCount != count || _pwLastModifiedcache.Any(c => c.Key.LastModificationTime != c.Value))
            {
                _cache.Clear();
                _pwToLvicache.Clear();
                _pwLastModifiedcache.Clear();
                _liGroupsPreviousSelected = selectedGroup;
                _lastCount = count;
            }
        }

        private void BuildPwEntryToListViewItemCache()
        {
            foreach (ListViewItem lvi in _listView.Items)
            {
                var entry = ((PwListItem)lvi.Tag).Entry;
                if (!entry.IsExpired() && _plugin.TOTPEntryValidator.CanGenerateTOTP(entry))
                {
                    _pwToLvicache.TryAdd(entry, lvi);
                    _pwLastModifiedcache.TryAdd(entry, entry.LastModificationTime);
                }
            }
        }

        private string GenerateOtpColumnValue(PwEntry passwordEntry)
        {
            var totpGenerator = _cache.GetOrAdd(passwordEntry, (pe) => new TOTPProvider(_plugin.TOTPEntryValidator.SettingsGet(pe), _plugin.TimeCorrections));
            var timeRemaining = (_plugin.Settings.TOTPColumnTimerVisible
                ? string.Format(" ({0})", totpGenerator.Timer)
                : string.Empty);

            string code;
            if (totpGenerator.LastCode != null && totpGenerator.Timer >= 1)
            {
                code = totpGenerator.LastCode;
            }
            else
            {
                var key = Base32.Decode(_plugin.TOTPEntryValidator.SeedGet(passwordEntry).ReadString().ExtWithoutSpaces());
                code = totpGenerator.GenerateByByte(key);
            }

            return code + timeRemaining;
        }

        private int GetColumnIndex()
        {
            return KeePass.Program.Config.MainWindow.EntryListColumns.FindIndex(column => column.Type == AceColumnType.PluginExt && column.CustomName == Localization.Strings.TOTP);
        }
    }
}

