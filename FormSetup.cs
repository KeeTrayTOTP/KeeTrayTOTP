using System;
using System.Windows.Forms;

using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Form providing the Setup Wizard to create a new TOTP or modify and delete an existing one.
    /// </summary>
    internal partial class FormSetup : Form
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
        internal FormSetup(KeeTrayTOTPExt Plugin, PwEntry Entry)
        {
            _plugin = Plugin; //Defines variable from argument.
            entry = Entry; //Defines variable from argument.
            InitializeComponent(); //Form Initialization.
        }

        /// <summary>
        /// Currently displayed step.
        /// </summary>
        private int _CurrentStep;
        /// <summary>
        /// Currently displayed step.
        /// </summary>
        private int CurrentStep
        {
            get { return _CurrentStep; } //Returns the current step.
            set
            {
                _CurrentStep = value; //Sets the step to display.
                SetPanelVisibility(); //Displays the appropriate panel.
                switch (_CurrentStep) //Sets the buttons depending on the step.
                {
                    case 1:
                        //Welcome
                        if (_plugin.SettingsCheck(entry) || _plugin.SeedCheck(entry)) //Checks if existing totp
                        {
                            ButtonBack.Text = "&Delete"; //Makes the back button the delete button.
                            ButtonBack.Visible = true; //Shows the back button.
                            HelpProviderSetup.SetHelpString(ButtonBack, FormSetup_Localization.SetupDelete);
                        }
                        else
                        {
                            ButtonBack.Visible = false; //Hides the back button.
                        }
                        HelpProviderSetup.SetHelpString(ButtonNext, FormSetup_Localization.SetupNext);
                        //button next already focused
                        break;
                    case 2:
                        //Interval
                        ButtonBack.Text = "< &Back"; //Makes sure the back button is shown as the back button.
                        ButtonBack.Visible = true; //Shows the back button.
                        HelpProviderSetup.SetHelpString(ButtonBack, FormSetup_Localization.SetupPrevious);
                        //NumericIntervalSetup.Focus(); //Focus on the interval numeric.
                        break;
                    case 3:
                        //Length
                        //focusing on radio selects it, dont want that.
                        break;
                    case 4:
                        //Seed
                        TextBoxSeedSetup.Focus(); //Focus on the seed textbox.
                        break;
                    case 5:
                        //Time Correction
                        ButtonNext.Text = "&Next >"; //Makes sure the next button is shown as the next button.
                        HelpProviderSetup.SetHelpString(ButtonNext, FormSetup_Localization.SetupNext);
                        ComboBoxTimeCorrectionSetup.Focus(); //Focus on the time correction combobox.
                        break;
                    case 6:
                        //Proceed
                        ButtonBack.Enabled = true; //Shows the back button.
                        ButtonNext.Text = "&Proceed"; //Makes the next button the proceed button.
                        ButtonCancel.Enabled = true; //Enables the cancel button.
                        HelpProviderSetup.SetHelpString(ButtonNext, FormSetup_Localization.SetupProceed);
                        //button next already focused
                        break;
                    case 7:
                        //Confirmation
                        ButtonBack.Enabled = false; //Hides the back button.
                        ButtonNext.Text = "&Finish"; //Makes the next button the finish button.
                        ButtonCancel.Enabled = false; //Disables the cancel button.
                        HelpProviderSetup.SetHelpString(ButtonNext, FormSetup_Localization.SetupFinnish);
                        //button next already focused
                        break;
                }
            }
        }

        /// <summary>
        /// Displays the appropriate panel.
        /// </summary>
        private void SetPanelVisibility()
        {
            PanelStartSetup.Visible = CurrentStep == 1;
            PanelIntervalSetup.Visible = CurrentStep == 2;
            PanelLengthSetup.Visible = CurrentStep == 3;
            PanelSeedSetup.Visible = CurrentStep == 4;
            PanelTimeCorrectionSetup.Visible = CurrentStep == 5;
            PanelReadySetup.Visible = CurrentStep == 6;
            PanelFinishSetup.Visible = CurrentStep == 7;
        }

        /// <summary>
        /// Windows Form Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSetup_Load(object sender, EventArgs e)
        {
            Text = TrayTOTP_Plugin_Localization.strSetup + TrayTOTP_Plugin_Localization.strSpaceDashSpace + TrayTOTP_Plugin_Localization.strTrayTOTPPlugin; //Set form's name using constants.
            if (_plugin.SettingsCheck(entry)) //Checks the the totp settings exists.
            {
                string[] Settings = _plugin.SettingsGet(entry); //Gets the the existing totp settings.
                bool ValidInterval = false; bool ValidLength = false; bool ValidUrl = false;
                _plugin.SettingsValidate(entry, out ValidInterval, out ValidLength, out ValidUrl); //Validates the settings value.
                if (ValidInterval) NumericIntervalSetup.Value = Convert.ToDecimal(Settings[0]); //Checks if interval is valid and sets interval numeric to the setting value.
                if (ValidLength) //Checks if length is valid.
                {
                    RadioButtonLength6Setup.Checked = Settings[1] == "6"; //Sets length radio 6 to checked if the setting value is 6.
                    RadioButtonLength8Setup.Checked = Settings[1] == "8"; //Sets length radio 8 to checked if the setting value is 8.
                }
                if (ValidUrl) ComboBoxTimeCorrectionSetup.Text = Settings[2]; //Checks if url is valid and sets time correction textbox to the setting value.
            }
            if (_plugin.SeedCheck(entry)) TextBoxSeedSetup.Text = _plugin.SeedGet(entry).ReadString(); //Checks if the seed exists and sets seed textbox to the seed value.
            ComboBoxTimeCorrectionSetup.Items.AddRange(_plugin.TimeCorrections.ToComboBox()); //Gets existings time corrections and adds them in the combobox.
            CurrentStep = 1; //Initial step.
        }

        /// <summary>
        /// Windows Forms Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Nothing
        }

        /// <summary>
        /// Button that moves forward in the setup steps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonNext_Click(object sender, EventArgs e)
        {
            switch (_CurrentStep)
            {
                case 1:
                    //Welcome
                    break;
                case 2:
                    //Interval
                    ErrorProviderSetup.SetError(NumericIntervalSetup, string.Empty);
                    if ((NumericIntervalSetup.Value < 1) || (NumericIntervalSetup.Value > 60)) ErrorProviderSetup.SetError(NumericIntervalSetup, string.Format(FormSetup_Localization.SetupInterval1to60, Environment.NewLine));
                    if (ErrorProviderSetup.GetError(NumericIntervalSetup) != string.Empty) return;
                    break;
                case 3:
                    //Length
                    ErrorProviderSetup.SetError(RadioButtonLength8Setup, string.Empty);
                    if (!RadioButtonLength6Setup.Checked && !RadioButtonLength8Setup.Checked) ErrorProviderSetup.SetError(RadioButtonLength8Setup, FormSetup_Localization.SetupLengthMandatory);
                    if (ErrorProviderSetup.GetError(RadioButtonLength8Setup) != string.Empty) return;
                    break;
                case 4:
                    //Seed
                    ErrorProviderSetup.SetError(TextBoxSeedSetup, string.Empty);
                    if (TextBoxSeedSetup.Text == string.Empty) ErrorProviderSetup.SetError(TextBoxSeedSetup, FormSetup_Localization.SetupSeedCantBeEmpty);
                    string InvalidBase32Chars;
                    if (!TextBoxSeedSetup.Text.ExtWithoutSpaces().ExtIsBase32(out InvalidBase32Chars)) ErrorProviderSetup.SetError(TextBoxSeedSetup, FormSetup_Localization.SetupInvalidCharacter + "(" + InvalidBase32Chars + ")!");
                    if (ErrorProviderSetup.GetError(TextBoxSeedSetup) != string.Empty) return;
                    break;
                case 5:
                    //Time Correction
                    ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, string.Empty);
                    if (ComboBoxTimeCorrectionSetup.Text != string.Empty && !ComboBoxTimeCorrectionSetup.Text.StartsWith("http")) ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, FormSetup_Localization.SetupUrlMustContainHttp);
                    if (ComboBoxTimeCorrectionSetup.Text != string.Empty && !ComboBoxTimeCorrectionSetup.Text.Contains("://")) ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, FormSetup_Localization.SetupInvalidUrl);
                    if (ComboBoxTimeCorrectionSetup.Text.Contains(";")) ErrorProviderSetup.SetError(ComboBoxTimeCorrectionSetup, FormSetup_Localization.SetupInvalidCharacter + " (;)");
                    if (ErrorProviderSetup.GetError(ComboBoxTimeCorrectionSetup) != string.Empty) return;
                    break;
                case 6:
                    //Proceed
                    try
                    {
                        entry.CreateBackup(_plugin.m_host.MainWindow.ActiveDatabase);
                        entry.Strings.Set(_plugin.m_host.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, TrayTOTP_Plugin_Localization.setdef_string_TOTPSeed_StringName), new ProtectedString(true, TextBoxSeedSetup.Text));
                        entry.Strings.Set(_plugin.m_host.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, TrayTOTP_Plugin_Localization.setdef_string_TOTPSettings_StringName), new ProtectedString(false, NumericIntervalSetup.Value.ToString() + ";" + (RadioButtonLength6Setup.Checked ? "6" : (RadioButtonLength8Setup.Checked ? "8" : "6")) + (ComboBoxTimeCorrectionSetup.Text != string.Empty ? ";" : string.Empty) + ComboBoxTimeCorrectionSetup.Text));
                        entry.Touch(true);
                        _plugin.m_host.MainWindow.ActiveDatabase.Modified = true;
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                        return;
                    }
                    break;
                case 7:
                    //Confirmation
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
            }
            CurrentStep++;
        }

        /// <summary>
        /// Button that moves back in the setup steps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBack_Click(object sender, EventArgs e)
        {
            switch (CurrentStep)
            {
                case 1:
                    //Deletion
                    if (MessageBox.Show(FormSetup_Localization.SetupMessageAskDeleteCurrentEntry, TrayTOTP_Plugin_Localization.strTrayTOTPPlugin, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                    return;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
            }
            CurrentStep--;
        }

        /// <summary>
        /// Button that closes the form and return a dialog result cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            //Dialog Result Cancel
        }

        /// <summary>
        /// Checkbox that enables the seed textbox to display the seed in plain text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxSeedVisibility_CheckedChanged(object sender, EventArgs e)
        {
            TextBoxSeedSetup.UseSystemPasswordChar = !CheckBoxSeedVisibility.Checked; //Displays the seed's textbox data to the user in plain text or hides it.
        }
    }
}
