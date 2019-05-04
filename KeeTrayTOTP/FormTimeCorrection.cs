using System;
using System.ComponentModel;
using System.Windows.Forms;
using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Form providing the validation of a Time Correction URL.
    /// </summary>
    internal partial class FormTimeCorrection : Form
    {
        /// <summary>
        /// Plugin Host.
        /// </summary>
        private readonly KeeTrayTOTPExt plugin;

        /// <summary>
        /// Windows Form Constructor when creating new Time Correction.
        /// </summary>
        /// <param name="Plugin">Plugin Host.</param>
        internal FormTimeCorrection(KeeTrayTOTPExt Plugin)
        {
            plugin = Plugin; //Defines variable from argument.
            InitializeComponent(); //Form Initialization.
        }
        /// <summary>
        /// Windows Form Constructor when modifying existing Time Correction.
        /// </summary>
        /// <param name="Plugin">Plugin Host.</param>
        /// <param name="URL">URL to modify.</param>
        internal FormTimeCorrection(KeeTrayTOTPExt Plugin, string URL)
        {
            plugin = Plugin; //Defines variable from argument.
            InitializeComponent(); //Form Initialization.
            ComboBoxUrlTimeCorrection.Text = URL; //Defines default value from argument.
        }

        /// <summary>
        /// TimeCorrection Provider that validates the URL.
        /// </summary>
        private volatile TimeCorrectionProvider TestTimeCorrection;

        /// <summary>
        /// Windows Form Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormTimeCorrection_Load(object sender, EventArgs e)
        {
            Text = TrayTOTP_Plugin_Localization.strTimeCorrection + TrayTOTP_Plugin_Localization.strSpaceDashSpace + TrayTOTP_Plugin_Localization.strTrayTOTPPlugin; //Sets the form's display text.
            if (plugin.m_host.MainWindow.ActiveDatabase.IsOpen)
            {
                foreach (var pe in plugin.m_host.MainWindow.ActiveDatabase.RootGroup.GetEntries(true)) //Goes through all entries to find existing urls but excludes existing time corrections.
                {
                    if (plugin.SettingsCheck(pe)) //Checks that settings exists for this entry.
                    {
                        string[] Settings = plugin.SettingsGet(pe); //Gets the entry's totp settings.
                        bool ValidInterval = false; bool ValidLength = false; bool ValidUrl = false;
                        if (plugin.SettingsValidate(pe, out ValidInterval, out ValidLength, out ValidUrl)) //Validates the settings.
                        {
                            if (ValidUrl) //Makes sure the URL is also valid.
                            {
                                if (!ComboBoxUrlTimeCorrection.Items.Contains(Settings[2]) && plugin.TimeCorrections[Settings[2]] == null) //Checks if not already in combobox or is an existing time correction.
                                {
                                    ComboBoxUrlTimeCorrection.Items.Add(Settings[2]); //Adds the URL to the combobox for quick adding.
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Windows Form Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormTimeCorrection_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WorkerWaitForCheck.IsBusy) //Checks if the worker is busy.
            {
                ButtonCancel.Enabled = false; //Disables the cancel button to prevent multiple cancels.
                WorkerWaitForCheck.CancelAsync(); //Asks the worker to cancel the thread.
                e.Cancel = true; //Prevents the form from closing just yet.
            }
        }

        /// <summary>
        /// Thread that wait for the Time Correction Validation to complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkerWaitForCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            TestTimeCorrection = new TimeCorrectionProvider(ComboBoxUrlTimeCorrection.Text); //Creates a new Time Correction to validate the desired URL.
            while (!WorkerWaitForCheck.CancellationPending) //Waits for the validation to end or a cancellation.
            {
                System.Threading.Thread.Sleep(100); //Waits
                if (TestTimeCorrection.LastUpdateDateTime != DateTime.MinValue) //Checks if the validation has completed.
                {
                    if (TestTimeCorrection.LastUpdateSucceded) //Checks if the validation has succeeded.
                    {
                        e.Result = "success"; //Returns the validation result success.
                        return; //Exits the loop as the validation was successful.
                    }
                    e.Result = "fail"; //Returns the validation result failed.
                    return; //Exits the loop as the validation was a failure.
                }
            }
            e.Cancel = true; //Returns the cancellation result if a cancellation was requested before the validation was completed.
        }

        /// <summary>
        /// When worker thread is completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkerWaitForCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled) //Checks if thread was cancelled.
            {
                if (e.Result.ToString() == "success") //Checks if thread was a success.
                {
                    PictureBoxTimeCorrection.Image = ImageListErrorProvider.Images[0]; //Displays success icon.
                    LabelStatusTimeCorrection.Text = FormTimeCorrection_Localization.TcSucces; //Diplays success message.
                    ButtonOK.Enabled = true; //Enables the user to add the time correction.
                    ButtonVerify.Visible = false; //Hides the verification button.
                }
                if (e.Result.ToString() == "fail") //Checks if thread has failed.
                {
                    PictureBoxTimeCorrection.Image = ImageListErrorProvider.Images[3]; //Displays failure icon.
                    LabelStatusTimeCorrection.Text = FormTimeCorrection_Localization.TcConnectionFailed; //Diplays failure message.
                    ButtonVerify.Enabled = true; //Enables the user to retry the URL validation.
                    ComboBoxUrlTimeCorrection.Enabled = true; //Enables the user to modify the last URL that was attempted to validate.
                }
            }
            else
            {
                PictureBoxTimeCorrection.Image = ImageListErrorProvider.Images[2]; //Displays cancellation icon.
                LabelStatusTimeCorrection.Text = FormTimeCorrection_Localization.TcVerificationCancelled; //Diplays cancellation message.
                ButtonVerify.Enabled = true; //Enables the user to retry the URL validation.
                ComboBoxUrlTimeCorrection.Enabled = true; //Enables the user to modify the last URL that was attempted to validate.
            }
            ButtonCancel.Enabled = true; //Enables the user to discard the last URL that was attempted to validate. 
        }

        /// <summary>
        /// Button that begin the validation of the entered URL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonVerify_Click(object sender, EventArgs e)
        {
            ErrorProviderTimeCorrection.SetError(ComboBoxUrlTimeCorrection, string.Empty); //Clears input errors.
            if (!ComboBoxUrlTimeCorrection.Text.StartsWith("http")) ErrorProviderTimeCorrection.SetError(ComboBoxUrlTimeCorrection, FormTimeCorrection_Localization.TcUrlMustContainHttp); //Verifies if the URL is valid.
            if (!ComboBoxUrlTimeCorrection.Text.Contains("://")) ErrorProviderTimeCorrection.SetError(ComboBoxUrlTimeCorrection, FormTimeCorrection_Localization.TcUrlInvalid); //Verifies if the URL is valid.
            if (plugin.TimeCorrections[ComboBoxUrlTimeCorrection.Text] != null) ErrorProviderTimeCorrection.SetError(ComboBoxUrlTimeCorrection, FormTimeCorrection_Localization.TcUrlExists); //Verifies if the URL is existing.
            if (ErrorProviderTimeCorrection.GetError(ComboBoxUrlTimeCorrection) != string.Empty) return; //Prevents the validation if input has an error.
            PictureBoxTimeCorrection.Image = ImageListErrorProvider.Images[1]; //Displays working icon.
            LabelStatusTimeCorrection.Text = FormTimeCorrection_Localization.TcPleaseWaitVerifying; //Diplays attempt message.
            ButtonVerify.Enabled = false; //Prevents the user to retry the URL validation.
            ComboBoxUrlTimeCorrection.Enabled = false; //Prevents the user to modify the URL.
            WorkerWaitForCheck.RunWorkerAsync(); //Starts the worker that handles the thread that handles the validation attempt.
        }

        /// <summary>
        /// Button that closes the form and returns a dialog result OK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            //Dialog Result = OK
        }

        /// <summary>
        /// Button that closes the form and returns a dialog result Cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            //Dialog Result = Cancel
        }
    }
}
