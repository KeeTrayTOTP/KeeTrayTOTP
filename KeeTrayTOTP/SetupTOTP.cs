using System;
using System.Windows.Forms;
using KeePass.UI;
using KeePassLib;
using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP
{
    public partial class SetupTOTP : Form
    {
        /// <summary>
        /// Plugin Host.
        /// </summary>
        private readonly KeeTrayTOTPExt _plugin;
        /// <summary>
        /// Current entry's reference.
        /// </summary>
        private readonly PwEntry _entry;

        /// <summary>
        /// Windows Form Constructor.
        /// </summary>
        /// <param name="plugin">Plugin Host.</param>
        /// <param name="entry">The Keepass <see cref="PwEntry"/> for which the TOTP Setup is called.</param>
        internal SetupTOTP(KeeTrayTOTPExt plugin, PwEntry entry)
        {
            _plugin = plugin; //Defines variable from argument.
            _entry = entry; //Defines variable from argument.
            InitializeComponent(); //Form Initialization.
        }

        private void SetupTOTP_Load(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            Text = Localization.Strings.Setup + " - " + Localization.Strings.TrayTOTPPlugin; //Set form's name using constants.

            var totpSettings = _plugin.TOTPEntryValidator.ReadAsKeyUri(_entry);

            if (totpSettings != null) //Checks the the totp settings exists.
            {
                TextBoxSeedSetup.Text = totpSettings.Secret;
                
                NumericIntervalSetup.Value = totpSettings.Period;

                // Select the correct radio button
                RadioButtonLength6Setup.Checked = totpSettings.Digits == 6;
                RadioButtonLength7Setup.Checked = totpSettings.Digits == 7;
                RadioButtonLength8Setup.Checked = totpSettings.Digits == 8;
                RadioButtonSteamFormatSetup.Checked = totpSettings.Digits == 5;
                ComboBoxTimeCorrectionSetup.Text = totpSettings.TimeCorrectionUrl != null ? totpSettings.TimeCorrectionUrl.AbsoluteUri : null;

                DeleteSetupButton.Visible = true; //Shows the back button.
                HelpProviderSetup.SetHelpString(DeleteSetupButton, Localization.Strings.SetupDelete);
            }
            else
            {
                DeleteSetupButton.Visible = false; //Hides the back button.
            }

            ComboBoxTimeCorrectionSetup.Items.AddRange(_plugin.TimeCorrections.ToComboBox()); // Gets existing time corrections and adds them in the combobox.

            HelpProviderSetup.SetHelpString(FinishSetupButton, Localization.Strings.SetupFinish);

            ErrorProviderSetup.SetError(TextBoxSeedSetup, string.Empty);
            ErrorProviderSetup.SetError(NumericIntervalSetup, string.Empty);
            ErrorProviderSetup.SetError(RadioButtonLength8Setup, string.Empty);
            ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, string.Empty);
        }

        private void CheckBoxSeedVisibility_CheckedChanged(object sender, EventArgs e)
        {
            TextBoxSeedSetup.UseSystemPasswordChar = !CheckBoxSeedVisibility.Checked; //Displays the seed's textbox data to the user in plain text or hides it.
        }

        /// <summary>
        /// Button that moves forward in the setup steps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFinish_Click(object sender, EventArgs e)
        {
            string invalidBase32Chars;

            // TOTP Seed field
            if (TextBoxSeedSetup.Text == string.Empty) //If no TOTP Seed
            {
                ErrorProviderSetup.SetError(TextBoxSeedSetup, Localization.Strings.SetupSeedCantBeEmpty);
            }
            else if (Base32.HasInvalidPadding(TextBoxSeedSetup.Text.ExtWithoutSpaces()))
            {
                ErrorProviderSetup.SetError(TextBoxSeedSetup, Localization.Strings.SetupInvalidPadding);
            }
            else if (!TextBoxSeedSetup.Text.ExtWithoutSpaces().IsBase32(out invalidBase32Chars))
            {
                ErrorProviderSetup.SetError(TextBoxSeedSetup, Localization.Strings.SetupInvalidCharacter + "(" + invalidBase32Chars + ")");
            }
            else
            {
                ErrorProviderSetup.SetError(TextBoxSeedSetup, string.Empty);
            }

            // Interval field
            if ((NumericIntervalSetup.Value < 1) || (NumericIntervalSetup.Value > 180))
            {
                ErrorProviderSetup.SetError(NumericIntervalSetup, string.Format(Localization.Strings.SetupInterval, Environment.NewLine));
            }
            else
            {
                ErrorProviderSetup.SetError(NumericIntervalSetup, string.Empty);
            }

            // Format/Interval radios
            if (!RadioButtonLength6Setup.Checked && !RadioButtonLength7Setup.Checked && !RadioButtonLength8Setup.Checked && !RadioButtonSteamFormatSetup.Checked)
            {
                ErrorProviderSetup.SetError(RadioButtonLength8Setup, Localization.Strings.SetupLengthMandatory);
            }
            else
            {
                ErrorProviderSetup.SetError(RadioButtonLength8Setup, string.Empty);
            }

            // Time Correction Field
            if (ComboBoxTimeCorrectionSetup.Text != string.Empty)
            {
                string uriName = ComboBoxTimeCorrectionSetup.Text;

                Uri uriResult;
                bool validUrl = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (!validUrl)
                {
                    ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, Localization.Strings.SetupInvalidUrl);
                }
                else
                {
                    ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, string.Empty);
                }
            }

            if (ErrorProviderSetup.GetError(TextBoxSeedSetup) != string.Empty ||
                ErrorProviderSetup.GetError(NumericIntervalSetup) != string.Empty ||
                ErrorProviderSetup.GetError(ComboBoxTimeCorrectionSetup) != string.Empty ||
                ErrorProviderSetup.GetError(ComboBoxTimeCorrectionSetup) != string.Empty)
            {
                return;
            }

            try
            {
                _entry.CreateBackup(_plugin.PluginHost.MainWindow.ActiveDatabase);

                // try to get the current keyUri / settings: dit crasht bij nieuwe entries!
                var keyUri = _plugin.TOTPEntryValidator.ReadAsKeyUri(_entry);

                // Change the settings
                if (RadioButtonLength6Setup.Checked)
                {
                    keyUri.Digits = 6;
                }
                else if (RadioButtonLength7Setup.Checked)
                {
                    keyUri.Digits = 7;
                }
                else if (RadioButtonLength8Setup.Checked)
                {
                    keyUri.Digits = 8;
                }
                else if (RadioButtonSteamFormatSetup.Checked)
                {
                    keyUri.Digits = 5;
                }

                keyUri.Secret = TextBoxSeedSetup.Text;
                keyUri.Period = (int)NumericIntervalSetup.Value;
                Uri timecorrectionUri;
                if (string.IsNullOrWhiteSpace(ComboBoxTimeCorrectionSetup.Text) && Uri.TryCreate(ComboBoxTimeCorrectionSetup.Text, UriKind.Absolute, out timecorrectionUri))
                {
                    keyUri.TimeCorrectionUrl = timecorrectionUri;
                }
                else
                {
                    keyUri.TimeCorrectionUrl = null;
                }

                _plugin.TOTPEntryValidator.SetKeyUri(_entry, keyUri);

                _entry.Touch(true);

                _plugin.PluginHost.MainWindow.ActiveDatabase.Modified = true;
                _plugin.ResetLastSelectedGroup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Button that moves back in the setup steps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Localization.Strings.SetupMessageAskDeleteCurrentEntry, Localization.Strings.TrayTOTPPlugin, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _entry.CreateBackup(_plugin.PluginHost.MainWindow.ActiveDatabase);
                    _entry.Strings.Remove(_plugin.Settings.TOTPSeedStringName);
                    _entry.Strings.Remove(_plugin.Settings.TOTPSettingsStringName);
                    _entry.Touch(true);
                    _plugin.PluginHost.MainWindow.ActiveDatabase.Modified = true;
                }
                catch
                {
                    // Ignore for now
                }
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void SetupTOTP_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }
    }
}
