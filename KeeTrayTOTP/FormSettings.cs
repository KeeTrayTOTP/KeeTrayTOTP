using System;
using System.ComponentModel;
using System.Windows.Forms;
using KeePass.UI;

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

            Text = Localization.Strings.Settings + @" - " + Localization.Strings.TrayTOTPPlugin; // Set form's name using constants.
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
            CheckBoxShowCopyTOTPEntryMenu.Checked = _plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_EntryContextCopy_Visible, true);
            CheckBoxShowSetupTOTPEntryMenu.Checked = _plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_EntryContextSetup_Visible, true);
            CheckBoxShowTOTPEntriesTrayMenu.Checked = _plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_NotifyContext_Visible, true);
            CheckBoxTrimTrayText.Checked = _plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, false);
            if (WorkerLoad.CancellationPending) { e.Cancel = true; return; }

            // TOTP Column
            CheckBoxTOTPColumnClipboard.Checked = _plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnCopy_Enable, true);
            CheckBoxTOTPColumnTimer.Checked = _plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnTimer_Visible, true);
            if (WorkerLoad.CancellationPending) { e.Cancel = true; return; }

            // Auto-Type
            CheckBoxAutoType.Checked = _plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_AutoType_Enable, true);
            TextBoxAutoTypeFieldName.Text = _plugin.PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_AutoType_FieldName, KeeTrayTOTPExt.setdef_string_AutoType_FieldName);
            if (WorkerLoad.CancellationPending) { e.Cancel = true; return; }

            // Time Correction
            CheckBoxTimeCorrection.Checked = _plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TimeCorrection_Enable, false);
            NumericTimeCorrectionInterval.Value = Convert.ToDecimal(_plugin.PluginHost.CustomConfig.GetULong(KeeTrayTOTPExt.setname_ulong_TimeCorrection_RefreshTime, KeeTrayTOTPExt.setdef_ulong_TimeCorrection_RefreshTime));
            ListViewTimeCorrectionList.Items.AddRange(_plugin.TimeCorrections.ToLvi());
            if (WorkerLoad.CancellationPending) { e.Cancel = true; return; }

            // Storage
            if (_plugin.PluginHost.MainWindow.ActiveDatabase.IsOpen)
            {
                foreach (var pe in _plugin.PluginHost.MainWindow.ActiveDatabase.RootGroup.GetEntries(true))
                {
                    foreach (var str in pe.Strings)
                    {
                        if (!ComboBoxTOTPSeedStringName.Items.Contains(str.Key))
                        {
                            ComboBoxTOTPSeedStringName.Items.Add(str.Key);
                            ComboBoxTOTPSettingsStringName.Items.Add(str.Key);
                        }
                    }
                }
            }
            ComboBoxTOTPSeedStringName.Text = _plugin.PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed);
            ComboBoxTOTPSettingsStringName.Text = _plugin.PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings);
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
            _plugin.PluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_EntryContextCopy_Visible, CheckBoxShowCopyTOTPEntryMenu.Checked);
            _plugin.PluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_EntryContextSetup_Visible, CheckBoxShowSetupTOTPEntryMenu.Checked);
            _plugin.PluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_NotifyContext_Visible, CheckBoxShowTOTPEntriesTrayMenu.Checked);
            _plugin.PluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_TrimTrayText, CheckBoxTrimTrayText.Checked);
            if (WorkerSave.CancellationPending) { e.Cancel = true; return; }

            //TOTP Column
            _plugin.PluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnCopy_Enable, CheckBoxTOTPColumnClipboard.Checked);
            _plugin.PluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnTimer_Visible, CheckBoxTOTPColumnTimer.Checked);
            if (WorkerSave.CancellationPending) { e.Cancel = true; return; }

            //Auto-Type
            _plugin.PluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_AutoType_Enable, CheckBoxAutoType.Checked);
            if (CheckBoxAutoTypeFieldName.Checked)
            {
                string oldAutoTypeFieldName = _plugin.PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_AutoType_FieldName, KeeTrayTOTPExt.setdef_string_AutoType_FieldName).ExtWithBrackets();
                string newAutoTypeFieldName = TextBoxAutoTypeFieldName.Text.ExtWithBrackets();
                KeePass.Util.Spr.SprEngine.FilterPlaceholderHints.Remove(oldAutoTypeFieldName);
                if (CheckBoxAutoTypeFieldRename.Checked) //Replace existing field of custom keystrokes from all entries and all groups
                {
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
                _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_AutoType_FieldName, TextBoxAutoTypeFieldName.Text);
                KeePass.Util.Spr.SprEngine.FilterPlaceholderHints.Add(newAutoTypeFieldName);
            }
            if (WorkerSave.CancellationPending) { e.Cancel = true; return; }

            //Time Correction
            _plugin.PluginHost.CustomConfig.SetBool(KeeTrayTOTPExt.setname_bool_TimeCorrection_Enable, CheckBoxTimeCorrection.Checked);
            _plugin.TimeCorrections.Enable = CheckBoxTimeCorrection.Checked;
            _plugin.PluginHost.CustomConfig.SetULong(KeeTrayTOTPExt.setname_ulong_TimeCorrection_RefreshTime, Convert.ToUInt64(NumericTimeCorrectionInterval.Value));
            KeeTrayTOTP.Libraries.TimeCorrectionProvider.Interval = Convert.ToInt16(NumericTimeCorrectionInterval.Value);
            _plugin.TimeCorrections.ResetThenAddRangeFromLvIs(ListViewTimeCorrectionList.Items);
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_TimeCorrection_List, _plugin.TimeCorrections.ToSetting());
            if (WorkerSave.CancellationPending) { e.Cancel = true; return; }

            //Storage
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, ComboBoxTOTPSeedStringName.Text);
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, ComboBoxTOTPSettingsStringName.Text);
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
            // Menus
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_bool_EntryContextCopy_Visible, null);
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_bool_EntryContextSetup_Visible, null);
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_bool_NotifyContext_Visible, null);

            // TOTP Column
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_bool_TOTPColumnCopy_Enable, null);
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_bool_TOTPColumnTimer_Visible, null);

            // Auto-Type
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_bool_AutoType_Enable, null);
            string oldAutoTypeFieldName = _plugin.PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_AutoType_FieldName, KeeTrayTOTPExt.setdef_string_AutoType_FieldName).ExtWithBrackets();
            string newAutoTypeFieldName = Localization.Strings.TOTP.ExtWithBrackets();
            KeePass.Util.Spr.SprEngine.FilterPlaceholderHints.Remove(oldAutoTypeFieldName);
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
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_AutoType_FieldName, null);
            KeePass.Util.Spr.SprEngine.FilterPlaceholderHints.Add(newAutoTypeFieldName);

            // Time Correction
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_bool_TimeCorrection_Enable, null);
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_ulong_TimeCorrection_RefreshTime, null);
            Libraries.TimeCorrectionProvider.Interval = Convert.ToInt16(KeeTrayTOTPExt.setdef_ulong_TimeCorrection_RefreshTime);
            _plugin.TimeCorrections.ResetThenAddRangeFromString(string.Empty);

            // Storage
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, null);
            _plugin.PluginHost.CustomConfig.SetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, null);
        }

        private void WorkerReset_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerLoad.RunWorkerAsync("Reset");
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }
    }
}
