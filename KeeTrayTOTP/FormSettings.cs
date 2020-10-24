using KeePass.UI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Form providing controls to customize the plugin's settings.
    /// </summary>
    internal partial class FormSettings : Form
    {
        /// <summary>
        /// Helper delegate for cross thread UI operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private delegate void SafeCallDelegate(object sender, DoWorkEventArgs e);

        /// <summary>
        /// Plugin Host.
        /// </summary>
        private readonly KeeTrayTOTPExt _plugin;

        /// <summary>
        /// Windows Form Constructor.
        /// </summary>
        /// <param name="plugin">Plugin Host.</param>
        internal FormSettings(KeeTrayTOTPExt plugin)
        {
            _plugin = plugin; //Defines variable from argument.
            InitializeComponent(); //Form Initialization.
        }

        /// <summary>
        /// Contains last network connection status. Is true by default to control user prompt.
        /// </summary>
        private bool _networkWasConnected = true;

        /// <summary>
        /// Windows Form Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSettings_Load(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            Text = Localization.Strings.Settings + " - " + Localization.Strings.TrayTOTPPlugin; // Set form's name using constants.
            Working(true, true); // Set controls depending on the state of action.
            WorkerLoad.RunWorkerAsync(); // Load Settings in form controls.
        }

        /// <summary>
        /// Windows Form Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WorkerLoad.IsBusy)
            {
                ButtonCancel.Enabled = false;
                WorkerLoad.CancelAsync();
                e.Cancel = true;
            }
            if (WorkerSave.IsBusy)
            {
                ButtonCancel.Enabled = false;
                WorkerSave.CancelAsync();
                e.Cancel = true;
            }
            if (WorkerReset.IsBusy)
            {
                ButtonCancel.Enabled = false;
                WorkerReset.CancelAsync();
                e.Cancel = true;
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Localization.Strings.SettingsAskResetDefaultValues, Localization.Strings.TrayTOTPPlugin, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Working(true, false); //Set controls depending on the state of action.
                WorkerReset.RunWorkerAsync();
            }
        }

        private void CheckBoxAutoType_CheckedChanged(object sender, EventArgs e)
        {
            GroupBoxAutoType.Enabled = CheckBoxAutoType.Checked;
        }

        private void CheckBoxAutoTypeFieldName_CheckedChanged(object sender, EventArgs e)
        {
            TextBoxAutoTypeFieldName.Enabled = CheckBoxAutoTypeFieldName.Checked;
            if (!CheckBoxAutoTypeFieldName.Checked)
            {
                CheckBoxAutoTypeFieldRename.Checked = false;
            }
        }

        private void CheckBoxAutoTypeFieldRename_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxAutoTypeFieldRename.Checked)
            {
                if (!_plugin.PluginHost.MainWindow.ActiveDatabase.IsOpen)
                {
                    CheckBoxAutoTypeFieldRename.Checked = false;
                    if (_plugin.PluginHost.MainWindow.IsFileLocked(null))
                    {
                        MessageBox.Show(Localization.Strings.SettingsCurrentDatabaseLocked, Localization.Strings.TrayTOTPPlugin, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    MessageBox.Show(Localization.Strings.SettingsOpenDatabaseRequired, Localization.Strings.TrayTOTPPlugin, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!CheckBoxAutoTypeFieldName.Checked)
                {
                    CheckBoxAutoTypeFieldRename.Checked = false;
                    MessageBox.Show(Localization.Strings.SettingsEnableFieldRename, Localization.Strings.TrayTOTPPlugin, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CheckBoxTimeCorrection_CheckedChanged(object sender, EventArgs e)
        {
            GroupBoxTimeCorrection.Enabled = CheckBoxTimeCorrection.Checked;
            GroupBoxTimeCorrectonList.Enabled = CheckBoxTimeCorrection.Checked;
        }

        private void TabControlSettings_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == "TabPageSync")
            {
                ToolStripButtonRefreshTimeCorrection_Click(null, null);
            }
        }

        private void ListViewTimeCorrectionList_DoubleClick(object sender, EventArgs e)
        {
            ToolStripButtonPropertiesTimeCorrection_Click(sender, e);
        }

        private void ToolStripButtonAddTimeCorrection_Click(object sender, EventArgs e)
        {
            var formTc = new FormTimeCorrection(_plugin);
            if (formTc.ShowDialog() == DialogResult.OK)
            {
                ListViewTimeCorrectionList.Items.Add(formTc.ComboBoxUrlTimeCorrection.Text, 0);
            }
        }

        private void ToolStripButtonPropertiesTimeCorrection_Click(object sender, EventArgs e)
        {
            if (ListViewTimeCorrectionList.SelectedItems.Count == 1)
            {
                ListViewItem thisItem = ListViewTimeCorrectionList.SelectedItems[0];
                var formTc = new FormTimeCorrection(_plugin, thisItem.Text);
                if (formTc.ShowDialog() == DialogResult.OK)
                {
                    thisItem.SubItems[0].Text = formTc.ComboBoxUrlTimeCorrection.Text;
                    thisItem.SubItems[1].Text = string.Empty;
                    thisItem.ImageIndex = 0;
                }
            }
        }

        private void ToolStripButtonRemoveTimeCorrection_Click(object sender, EventArgs e)
        {
            if (ListViewTimeCorrectionList.SelectedItems.Count == 1)
            {
                ListViewTimeCorrectionList.SelectedItems[0].Remove();
            }
        }

        private void ToolStripButtonRefreshTimeCorrection_Click(object sender, EventArgs e)
        {
            ListViewTimeCorrectionList.Items.Clear();
            ListViewTimeCorrectionList.Items.AddRange(_plugin.TimeCorrections.ToLvi());
            if (!_plugin.NetworkIsConnected)
            {
                if (_networkWasConnected)
                {
                    MessageBox.Show(string.Format(Localization.Strings.SettingsNoInternetDetected, Environment.NewLine), Localization.Strings.TrayTOTPPlugin, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ToolStripButtonAddTimeCorrection.Enabled = false;
                ToolStripButtonPropertiesTimeCorrection.Enabled = false;
                _networkWasConnected = false;
            }
            else
            {
                ToolStripButtonAddTimeCorrection.Enabled = true;
                ToolStripButtonPropertiesTimeCorrection.Enabled = true;
                _networkWasConnected = true;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (HasErrors())
            {
                return;
            }

            Working(true, true); //Set controls depending on the state of action.
            WorkerSave.RunWorkerAsync("OK");
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            //Dialog Result = Cancel
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            if (HasErrors())
            {
                return;
            }

            Working(true, true); //Set controls depending on the state of action.
            WorkerSave.RunWorkerAsync();
        }

        private bool HasErrors()
        {
            bool temp = false;
            ErrorProviderSettings.SetError(ButtonOK, string.Empty);
            ErrorProviderSettings.SetError(TextBoxAutoTypeFieldName, string.Empty);
            ErrorProviderSettings.SetError(ComboBoxTOTPSeedStringName, string.Empty);
            ErrorProviderSettings.SetError(ComboBoxTOTPSettingsStringName, string.Empty);
            if (TextBoxAutoTypeFieldName.Text.Contains("{") || TextBoxAutoTypeFieldName.Text.Contains("}"))
            {
                ErrorProviderSettings.SetError(TextBoxAutoTypeFieldName, Localization.Strings.SettingsInvalidCharacter);
            }

            if (ComboBoxTOTPSeedStringName.Text == ComboBoxTOTPSettingsStringName.Text)
            {
                ErrorProviderSettings.SetError(ComboBoxTOTPSeedStringName, Localization.Strings.SettingsInvalidNameSetting);
                ErrorProviderSettings.SetError(ComboBoxTOTPSettingsStringName, Localization.Strings.SettingsInvalidNameSeed);
            }
            if (ErrorProviderSettings.GetError(TextBoxAutoTypeFieldName) != string.Empty)
            {
                temp = true;
            }

            if (ErrorProviderSettings.GetError(ComboBoxTOTPSeedStringName) != string.Empty)
            {
                temp = true;
            }

            if (ErrorProviderSettings.GetError(ComboBoxTOTPSettingsStringName) != string.Empty)
            {
                temp = true;
            }

            if (temp)
            {
                ErrorProviderSettings.SetError(ButtonOK, Localization.Strings.SettingsErrors);
            }

            return temp;
        }

        private void Working(bool enable, bool cancellable)
        {
            UseWaitCursor = enable;
            TabControlSettings.Enabled = !enable;
            ButtonReset.Enabled = !enable;
            ButtonOK.Enabled = !enable;
            ButtonCancel.Enabled = cancellable;
            ButtonApply.Enabled = !enable;
        }

        private void WorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            if (InvokeRequired)
            {
                var safeCallDelegate = new SafeCallDelegate(WorkerLoad_DoWork);
                Invoke(safeCallDelegate, sender, e);
                return;
            }

            // Argument
            e.Result = e.Argument;

            // Menus
            CheckBoxShowCopyTOTPEntryMenu.Checked = _plugin.Settings.EntryContextCopyVisible;
            CheckBoxShowSetupTOTPEntryMenu.Checked = _plugin.Settings.EntryContextSetupVisible;
            CheckBoxShowTOTPEntriesTrayMenu.Checked = _plugin.Settings.NotifyContextVisible;
            CheckBoxEnableLegacyTrayMenuProvider.Checked = _plugin.Settings.LegacyTrayMenuProviderEnable;
            CheckBoxTrimTrayText.Checked = _plugin.Settings.TrimTrayText;
            if (WorkerLoad.CancellationPending) { e.Cancel = true; return; }

            // TOTP Column
            CheckBoxTOTPColumnClipboard.Checked = _plugin.Settings.TOTPColumnCopyEnable;
            CheckBoxTOTPColumnTimer.Checked = _plugin.Settings.TOTPColumnTimerVisible;
            if (WorkerLoad.CancellationPending) { e.Cancel = true; return; }

            // Auto-Type
            CheckBoxAutoType.Checked = _plugin.Settings.AutoTypeEnable;
            TextBoxAutoTypeFieldName.Text = _plugin.Settings.AutoTypeFieldName;
            if (WorkerLoad.CancellationPending) { e.Cancel = true; return; }

            // Time Correction
            CheckBoxTimeCorrection.Checked = _plugin.Settings.TimeCorrectionEnable;
            NumericTimeCorrectionInterval.Value = Convert.ToDecimal(_plugin.Settings.TimeCorrectionRefreshTime);
            ListViewTimeCorrectionList.Items.AddRange(_plugin.TimeCorrections.ToLvi());
            if (WorkerLoad.CancellationPending) { e.Cancel = true; return; }

            // Storage
            if (_plugin.PluginHost.MainWindow.ActiveDatabase.IsOpen)
            {
                foreach (var str in _plugin.PluginHost.MainWindow.ActiveDatabase.RootGroup.GetEntries(true).SelectMany(pe => pe.Strings))
                {
                    if (!ComboBoxTOTPSeedStringName.Items.Contains(str.Key))
                    {
                        ComboBoxTOTPSeedStringName.Items.Add(str.Key);
                        ComboBoxTOTPSettingsStringName.Items.Add(str.Key);
                    }
                }
            }
            ComboBoxTOTPSeedStringName.Text = _plugin.Settings.TOTPSeedStringName;
            ComboBoxTOTPSettingsStringName.Text = _plugin.Settings.TOTPSettingsStringName;
            if (WorkerLoad.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void WorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                Working(false, true); // Set controls depending on the state of action.
                if (e.Result != null && e.Result.ToString() == "Reset")
                {
                    MessageBox.Show(Localization.Strings.SettingsDefaultValuesRestored, Localization.Strings.TrayTOTPPlugin, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void WorkerSave_DoWork(object sender, DoWorkEventArgs e)
        {
            //Argument
            e.Result = e.Argument;

            //Menus
            _plugin.Settings.EntryContextCopyVisible = CheckBoxShowCopyTOTPEntryMenu.Checked;
            _plugin.Settings.EntryContextSetupVisible = CheckBoxShowSetupTOTPEntryMenu.Checked;
            _plugin.Settings.NotifyContextVisible = CheckBoxShowTOTPEntriesTrayMenu.Checked;
            _plugin.Settings.LegacyTrayMenuProviderEnable = CheckBoxEnableLegacyTrayMenuProvider.Checked;
            _plugin.Settings.TrimTrayText = CheckBoxTrimTrayText.Checked;

            if (WorkerSave.CancellationPending) { e.Cancel = true; return; }

            //TOTP Column
            _plugin.Settings.TOTPColumnCopyEnable = CheckBoxTOTPColumnClipboard.Checked;
            _plugin.Settings.TOTPColumnTimerVisible = CheckBoxTOTPColumnTimer.Checked;

            if (WorkerSave.CancellationPending) { e.Cancel = true; return; }

            //Auto-Type
            _plugin.Settings.AutoTypeEnable = CheckBoxAutoType.Checked;
            if (CheckBoxAutoTypeFieldName.Checked)
            {
                if (CheckBoxAutoTypeFieldRename.Checked) // Replace existing field of custom keystrokes from all entries and all groups
                {
                    string oldAutoTypeFieldName = _plugin.Settings.AutoTypeFieldName.ExtWithBrackets();
                    string newAutoTypeFieldName = TextBoxAutoTypeFieldName.Text.ExtWithBrackets();

                    SetupAutoType(oldAutoTypeFieldName, newAutoTypeFieldName);
                }
                _plugin.Settings.AutoTypeFieldName = TextBoxAutoTypeFieldName.Text;
            }
            if (WorkerSave.CancellationPending) { e.Cancel = true; return; }

            //Time Correction
            _plugin.Settings.TimeCorrectionEnable = CheckBoxTimeCorrection.Checked;
            _plugin.TimeCorrections.Enable = CheckBoxTimeCorrection.Checked;
            _plugin.Settings.TimeCorrectionRefreshTime = Convert.ToUInt64(NumericTimeCorrectionInterval.Value);
            KeeTrayTOTP.Libraries.TimeCorrectionProvider.Interval = Convert.ToInt16(NumericTimeCorrectionInterval.Value);
            _plugin.TimeCorrections.ResetThenAddRangeFromLvIs(ListViewTimeCorrectionList.Items);

            _plugin.Settings.TimeCorrectionList = _plugin.TimeCorrections.GetTimeCorrectionUrls();
            if (WorkerSave.CancellationPending) { e.Cancel = true; return; }

            //Storage
            _plugin.Settings.TOTPSeedStringName = ComboBoxTOTPSeedStringName.Text;
            _plugin.Settings.TOTPSettingsStringName = ComboBoxTOTPSettingsStringName.Text;

            if (WorkerSave.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void WorkerSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                Working(false, true); //Set controls depending on the state of action.
                if (e.Result != null && e.Result.ToString() == "OK")
                {
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void WorkerReset_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get a reference to old settings (for AutoType)
            string oldAutoTypeFieldName = _plugin.Settings.AutoTypeFieldName.ExtWithBrackets();

            // Reset the settings
            _plugin.Settings.Reset();

            // Apply the new settings
            Libraries.TimeCorrectionProvider.Interval = Convert.ToInt16(_plugin.Settings.TimeCorrectionRefreshTime);
            _plugin.TimeCorrections.ResetThenAddRangeFromString(string.Empty);

            // Auto Type
            string newAutoTypeFieldName = Localization.Strings.TOTP.ExtWithBrackets();
            SetupAutoType(oldAutoTypeFieldName, newAutoTypeFieldName);
        }

        private void SetupAutoType(string oldAutoTypeFieldName, string newAutoTypeFieldName)
        {
            KeePass.Util.Spr.SprEngine.FilterPlaceholderHints.Remove(oldAutoTypeFieldName);
            KeePass.Util.Spr.SprEngine.FilterPlaceholderHints.Add(newAutoTypeFieldName);

            _plugin.PluginHost.MainWindow.ActiveDatabase.RootGroup.DefaultAutoTypeSequence = _plugin.PluginHost.MainWindow.ActiveDatabase.RootGroup.DefaultAutoTypeSequence.Replace(oldAutoTypeFieldName, newAutoTypeFieldName);
            foreach (var group in _plugin.PluginHost.MainWindow.ActiveDatabase.RootGroup.GetGroups(true))
            {
                group.DefaultAutoTypeSequence = group.DefaultAutoTypeSequence.Replace(oldAutoTypeFieldName, newAutoTypeFieldName);
                foreach (var pe in group.GetEntries(false))
                {
                    foreach (var association in pe.AutoType.Associations)
                    {
                        association.Sequence = association.Sequence.Replace(oldAutoTypeFieldName, newAutoTypeFieldName);
                    }
                }
            }
        }

        private void WorkerReset_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerLoad.RunWorkerAsync("Reset");
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private void CalculateRefreshNoticeVisibility(object sender, EventArgs e)
        {
            var showTotpEntriesTrayMenuChanged = (_plugin.Settings.NotifyContextVisible != CheckBoxShowTOTPEntriesTrayMenu.Checked);
            var showLegacyTotpEntriesTrayMenuChanged = (_plugin.Settings.LegacyTrayMenuProviderEnable != CheckBoxEnableLegacyTrayMenuProvider.Checked);

            LabelRestartRequired.Visible = showTotpEntriesTrayMenuChanged || showLegacyTotpEntriesTrayMenuChanged;
        }
    }
}
