using System;
using System.Windows.Forms;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Security;
using KeeTrayTOTP.Helpers;
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

            if (_plugin.TOTPEntryValidator.HasSeed(_entry)) //Checks the the totp settings exists.
            {
                string[] settings = _plugin.TOTPEntryValidator.SettingsGet(_entry); //Gets the the existing totp settings.
                bool validInterval;
                bool validLength;
                bool validUrl;
                _plugin.TOTPEntryValidator.SettingsValidate(_entry, out validInterval, out validLength, out validUrl); //Validates the settings value.
                if (validInterval)
                {
                    NumericIntervalSetup.Value = Convert.ToDecimal(settings[0]); //Checks if interval is valid and sets interval numeric to the setting value.
                }

                if (validLength) //Checks if length is valid.
                {
                    // Select the correct radio button
                    RadioButtonLength6Setup.Checked = settings[1] == "6";
                    RadioButtonLength7Setup.Checked = settings[1] == "7";
                    RadioButtonLength8Setup.Checked = settings[1] == "8";
                    RadioButtonSteamFormatSetup.Checked = settings[1] == "S";
                }
                if (validUrl)
                {
                    ComboBoxTimeCorrectionSetup.Text = settings[2]; //Checks if url is valid and sets time correction textbox to the setting value.
                }

                DeleteSetupButton.Visible = true; //Shows the back button.
                HelpProviderSetup.SetHelpString(DeleteSetupButton, Localization.Strings.SetupDelete);
            }
            else
            {
                DeleteSetupButton.Visible = false; //Hides the back button.
            }

            if (_plugin.TOTPEntryValidator.HasSeed(_entry))
            {
                TextBoxSeedSetup.Text = _plugin.TOTPEntryValidator.SeedGet(_entry).ReadString(); //Checks if the seed exists and sets seed textbox to the seed value.
            }

            ComboBoxTimeCorrectionSetup.Items.AddRange(_plugin.TimeCorrections.ToComboBox()); //Gets existings time corrections and adds them in the combobox.

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

                _entry.Strings.Set(_plugin.Settings.TOTPSeedStringName, new ProtectedString(true, TextBoxSeedSetup.Text));

                string format = "";

                if (RadioButtonLength6Setup.Checked)
                {
                    format = "6";
                }
                else if (RadioButtonLength7Setup.Checked)
                {
                    format = "7";
                }
                else if (RadioButtonLength8Setup.Checked)
                {
                    format = "8";
                }
                else if (RadioButtonSteamFormatSetup.Checked)
                {
                    format = "S";
                }

                var settings = new ProtectedString(false, NumericIntervalSetup.Value.ToString() + ";" + format + (ComboBoxTimeCorrectionSetup.Text != string.Empty ? ";" : string.Empty) + ComboBoxTimeCorrectionSetup.Text);
                _entry.Strings.Set(_plugin.Settings.TOTPSettingsStringName, settings);

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
