using System;
using System.Windows.Forms;
using KeePassLib;
using KeePassLib.Security;


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
        private readonly PwEntry entry;

        /// <summary>
        /// Windows Form Constructor.
        /// </summary>
        /// <param name="Plugin">Plugin Host.</param>
        internal SetupTOTP(KeeTrayTOTPExt Plugin, PwEntry Entry)
        {
            _plugin = Plugin; //Defines variable from argument.
            entry = Entry; //Defines variable from argument.
            InitializeComponent(); //Form Initialization.
        }

        private void SetupTOTP_Load(object sender, EventArgs e)
        {
            Text = TrayTOTP_Plugin_Localization.strSetup + TrayTOTP_Plugin_Localization.strSpaceDashSpace + TrayTOTP_Plugin_Localization.strTrayTOTPPlugin; //Set form's name using constants.

            if (_plugin.SettingsCheck(entry) || _plugin.SeedCheck(entry)) //Checks the the totp settings exists.
            {
                string[] Settings = _plugin.SettingsGet(entry); //Gets the the existing totp settings.
                bool ValidInterval;
                bool ValidLength;
                bool ValidUrl;
                _plugin.SettingsValidate(entry, out ValidInterval, out ValidLength, out ValidUrl); //Validates the settings value.
                if (ValidInterval) NumericIntervalSetup.Value = Convert.ToDecimal(Settings[0]); //Checks if interval is valid and sets interval numeric to the setting value.
                if (ValidLength) //Checks if length is valid.
                {
                    // Select the correct radio button
                    RadioButtonLength6Setup.Checked = Settings[1] == "6";
                    RadioButtonLength7Setup.Checked = Settings[1] == "7";
                    RadioButtonLength8Setup.Checked = Settings[1] == "8";
                    RadioButtonSteamFormatSetup.Checked = Settings[1] == "S";
                }
                if (ValidUrl) ComboBoxTimeCorrectionSetup.Text = Settings[2]; //Checks if url is valid and sets time correction textbox to the setting value.

                DeleteSetupButton.Visible = true; //Shows the back button.
                HelpProviderSetup.SetHelpString(DeleteSetupButton, SetupTOTP_Localization.SetupDelete);
            }
            else
            {
                DeleteSetupButton.Visible = false; //Hides the back button.
            }

            if (_plugin.SeedCheck(entry)) TextBoxSeedSetup.Text = _plugin.SeedGet(entry).ReadString(); //Checks if the seed exists and sets seed textbox to the seed value.
            ComboBoxTimeCorrectionSetup.Items.AddRange(_plugin.TimeCorrections.ToComboBox()); //Gets existings time corrections and adds them in the combobox.

            HelpProviderSetup.SetHelpString(FinishSetupButton, SetupTOTP_Localization.SetupFinish);

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
            string InvalidBase32Chars;

            // TOTP Seed field
            if (TextBoxSeedSetup.Text == string.Empty) //If no TOTP Seed
                ErrorProviderSetup.SetError(TextBoxSeedSetup, SetupTOTP_Localization.SetupSeedCantBeEmpty);
            else if (!TextBoxSeedSetup.Text.ExtWithoutSpaces().ExtIsBase32(out InvalidBase32Chars)) // TODO: Add support to other known formats
                ErrorProviderSetup.SetError(TextBoxSeedSetup, SetupTOTP_Localization.SetupInvalidCharacter + "(" + InvalidBase32Chars + ")!");
            else
                ErrorProviderSetup.SetError(TextBoxSeedSetup, string.Empty);

            // Interval field
            if ((NumericIntervalSetup.Value < 1) || (NumericIntervalSetup.Value > 180))
                ErrorProviderSetup.SetError(NumericIntervalSetup, string.Format(SetupTOTP_Localization.SetupInterval, Environment.NewLine));
            else
                ErrorProviderSetup.SetError(NumericIntervalSetup, string.Empty);

            // Format/Interval radios
            if (!RadioButtonLength6Setup.Checked && !RadioButtonLength7Setup.Checked && !RadioButtonLength8Setup.Checked && !RadioButtonSteamFormatSetup.Checked)
                ErrorProviderSetup.SetError(RadioButtonLength8Setup, SetupTOTP_Localization.SetupLengthMandatory);
            else
                ErrorProviderSetup.SetError(RadioButtonLength8Setup, string.Empty);

            // Time Correction Field
            if (ComboBoxTimeCorrectionSetup.Text != string.Empty)
            {
                string uriName = ComboBoxTimeCorrectionSetup.Text;

                Uri uriResult;
                bool validURL = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult)
                              && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (!validURL)
                    ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, SetupTOTP_Localization.SetupInvalidUrl);
                else
                    ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, string.Empty);
            }

            /* ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, SetupTOTP_Localization.SetupUrlMustContainHttp);
            if (ComboBoxTimeCorrectionSetup.Text.Contains(";"))
                ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, SetupTOTP_Localization.SetupInvalidCharacter + " (;)");
            */

            if (ErrorProviderSetup.GetError(TextBoxSeedSetup) != string.Empty ||
                ErrorProviderSetup.GetError(NumericIntervalSetup) != string.Empty ||
                ErrorProviderSetup.GetError(ComboBoxTimeCorrectionSetup) != string.Empty ||
                ErrorProviderSetup.GetError(ComboBoxTimeCorrectionSetup) != string.Empty)
                return;



            try
            {
                entry.CreateBackup(_plugin.m_host.MainWindow.ActiveDatabase);

                entry.Strings.Set(_plugin.m_host.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName,
                    TrayTOTP_Plugin_Localization.setdef_string_TOTPSeed_StringName),
                    new ProtectedString(true, TextBoxSeedSetup.Text)
                );

                string format = "";

                if (RadioButtonLength6Setup.Checked)
                    format = "6";
                else if (RadioButtonLength7Setup.Checked)
                    format = "7";
                else if (RadioButtonLength8Setup.Checked)
                    format = "8";
                else if (RadioButtonSteamFormatSetup.Checked)
                    format = "S";

                entry.Strings.Set(_plugin.m_host.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName,
                    TrayTOTP_Plugin_Localization.setdef_string_TOTPSettings_StringName),
                    new ProtectedString(false,
                        NumericIntervalSetup.Value.ToString() + ";" + format +
                        (ComboBoxTimeCorrectionSetup.Text != string.Empty ? ";" : string.Empty) + ComboBoxTimeCorrectionSetup.Text)
                    );

                entry.Touch(true);

                _plugin.m_host.MainWindow.ActiveDatabase.Modified = true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
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
            //if (_plugin.SettingsCheck(entry) || _plugin.SeedCheck(entry))
            if (MessageBox.Show(SetupTOTP_Localization.SetupMessageAskDeleteCurrentEntry, TrayTOTP_Plugin_Localization.strTrayTOTPPlugin, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    entry.CreateBackup(_plugin.m_host.MainWindow.ActiveDatabase);
                    entry.Strings.Remove(_plugin.m_host.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, TrayTOTP_Plugin_Localization.setdef_string_TOTPSeed_StringName));
                    entry.Strings.Remove(_plugin.m_host.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, TrayTOTP_Plugin_Localization.setdef_string_TOTPSettings_StringName));
                    entry.Touch(true);
                    _plugin.m_host.MainWindow.ActiveDatabase.Modified = true;
                }
                catch (Exception)
                {
                }
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
