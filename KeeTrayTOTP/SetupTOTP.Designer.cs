namespace KeeTrayTOTP
{
    partial class SetupTOTP
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupTOTP));
            this.TitleAboutLabel = new System.Windows.Forms.Label();
            this.PictureBoxAbout = new System.Windows.Forms.PictureBox();
            this.TextBoxSeedSetup = new System.Windows.Forms.TextBox();
            this.SeedDescriptionSetupLabel = new System.Windows.Forms.Label();
            this.InfoPanel = new System.Windows.Forms.Panel();
            this.StartSetupLabel = new System.Windows.Forms.Label();
            this.SeedPanel = new System.Windows.Forms.Panel();
            this.SeedSetupLabel = new System.Windows.Forms.Label();
            this.CheckBoxSeedVisibility = new System.Windows.Forms.CheckBox();
            this.IntervalSecondsSetupLabel = new System.Windows.Forms.Label();
            this.NumericIntervalSetup = new System.Windows.Forms.NumericUpDown();
            this.IntervalSetupLabel = new System.Windows.Forms.Label();
            this.IntervalDescriptionSetupLabel = new System.Windows.Forms.Label();
            this.IntervalPanel = new System.Windows.Forms.Panel();
            this.RadioButtonLength8Setup = new System.Windows.Forms.RadioButton();
            this.RadioButtonLength7Setup = new System.Windows.Forms.RadioButton();
            this.RadioButtonLength6Setup = new System.Windows.Forms.RadioButton();
            this.FormatSetupLabel = new System.Windows.Forms.Label();
            this.FormatDescriptionSetupLabel = new System.Windows.Forms.Label();
            this.LengthPanel = new System.Windows.Forms.Panel();
            this.RadioButtonSteamFormatSetup = new System.Windows.Forms.RadioButton();
            this.ComboBoxTimeCorrectionSetup = new System.Windows.Forms.ComboBox();
            this.TimeCorrectionSetupLabel = new System.Windows.Forms.Label();
            this.TimeCorrectionDescriptionSetupLabel = new System.Windows.Forms.Label();
            this.TimeCorrectionPanel = new System.Windows.Forms.Panel();
            this.CancelSetupButton = new System.Windows.Forms.Button();
            this.HelpProviderSetup = new System.Windows.Forms.HelpProvider();
            this.DeleteSetupButton = new System.Windows.Forms.Button();
            this.FinishSetupButton = new System.Windows.Forms.Button();
            this.ErrorProviderSetup = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbout)).BeginInit();
            this.InfoPanel.SuspendLayout();
            this.SeedPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericIntervalSetup)).BeginInit();
            this.IntervalPanel.SuspendLayout();
            this.LengthPanel.SuspendLayout();
            this.TimeCorrectionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProviderSetup)).BeginInit();
            this.SuspendLayout();
            // 
            // TitleAboutLabel
            // 
            this.TitleAboutLabel.BackColor = System.Drawing.Color.DarkOrange;
            this.TitleAboutLabel.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleAboutLabel.Location = new System.Drawing.Point(0, 0);
            this.TitleAboutLabel.Name = "TitleAboutLabel";
            this.TitleAboutLabel.Size = new System.Drawing.Size(365, 54);
            this.TitleAboutLabel.TabIndex = 11;
            this.TitleAboutLabel.TabStop = true;
            this.TitleAboutLabel.Text = "TOTP Setup Wizard";
            this.TitleAboutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PictureBoxAbout
            // 
            this.PictureBoxAbout.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.PictureBoxAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.PictureBoxAbout.Image = ((System.Drawing.Image)(resources.GetObject("PictureBoxAbout.Image")));
            this.PictureBoxAbout.Location = new System.Drawing.Point(311, 0);
            this.PictureBoxAbout.Name = "PictureBoxAbout";
            this.PictureBoxAbout.Size = new System.Drawing.Size(54, 54);
            this.PictureBoxAbout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PictureBoxAbout.TabIndex = 10;
            this.PictureBoxAbout.TabStop = false;
            // 
            // TextBoxSeedSetup
            // 
            this.TextBoxSeedSetup.Location = new System.Drawing.Point(82, 22);
            this.TextBoxSeedSetup.MaxLength = 255;
            this.TextBoxSeedSetup.Name = "TextBoxSeedSetup";
            this.TextBoxSeedSetup.Size = new System.Drawing.Size(245, 20);
            this.TextBoxSeedSetup.TabIndex = 0;
            this.TextBoxSeedSetup.Tag = "";
            this.TextBoxSeedSetup.UseSystemPasswordChar = true;
            // 
            // SeedDescriptionSetupLabel
            // 
            this.SeedDescriptionSetupLabel.Location = new System.Drawing.Point(5, 5);
            this.SeedDescriptionSetupLabel.Name = "SeedDescriptionSetupLabel";
            this.SeedDescriptionSetupLabel.Size = new System.Drawing.Size(322, 20);
            this.SeedDescriptionSetupLabel.TabIndex = 4;
            this.SeedDescriptionSetupLabel.TabStop = true;
            this.SeedDescriptionSetupLabel.Text = "The only format currently supported is Base32. Spaces are trimmed automatically.";
            // 
            // InfoPanel
            // 
            this.InfoPanel.Controls.Add(this.StartSetupLabel);
            this.InfoPanel.Location = new System.Drawing.Point(15, 60);
            this.InfoPanel.Name = "InfoPanel";
            this.InfoPanel.Size = new System.Drawing.Size(337, 60);
            this.InfoPanel.TabIndex = 7;
            this.InfoPanel.TabStop = true;
            // 
            // StartSetupLabel
            // 
            this.StartSetupLabel.Location = new System.Drawing.Point(0, 0);
            this.StartSetupLabel.Name = "StartSetupLabel";
            this.StartSetupLabel.Size = new System.Drawing.Size(337, 57);
            this.StartSetupLabel.TabIndex = 0;
            this.StartSetupLabel.TabStop = true;
            this.StartSetupLabel.Text = "This setup wizard will guide you through the creation or edition process of a Tim" +
    "e-based One Time Password for the selected entry.\r\n\r\nYou may remove a current TO" +
    "TP by using the \"Delete\" button.";
            // 
            // SeedPanel
            // 
            this.SeedPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SeedPanel.Controls.Add(this.SeedSetupLabel);
            this.SeedPanel.Controls.Add(this.TextBoxSeedSetup);
            this.SeedPanel.Controls.Add(this.SeedDescriptionSetupLabel);
            this.SeedPanel.Controls.Add(this.CheckBoxSeedVisibility);
            this.SeedPanel.Location = new System.Drawing.Point(10, 127);
            this.SeedPanel.Name = "SeedPanel";
            this.SeedPanel.Size = new System.Drawing.Size(345, 70);
            this.SeedPanel.TabIndex = 0;
            this.SeedPanel.TabStop = true;
            // 
            // SeedSetupLabel
            // 
            this.SeedSetupLabel.Location = new System.Drawing.Point(5, 21);
            this.SeedSetupLabel.Name = "SeedSetupLabel";
            this.SeedSetupLabel.Size = new System.Drawing.Size(71, 21);
            this.SeedSetupLabel.TabIndex = 0;
            this.SeedSetupLabel.TabStop = true;
            this.SeedSetupLabel.Text = "TOTP Seed :";
            this.SeedSetupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CheckBoxSeedVisibility
            // 
            this.CheckBoxSeedVisibility.AutoSize = true;
            this.CheckBoxSeedVisibility.Location = new System.Drawing.Point(82, 48);
            this.CheckBoxSeedVisibility.Name = "CheckBoxSeedVisibility";
            this.CheckBoxSeedVisibility.Size = new System.Drawing.Size(113, 17);
            this.CheckBoxSeedVisibility.TabIndex = 1;
            this.CheckBoxSeedVisibility.Tag = "";
            this.CheckBoxSeedVisibility.Text = "Show TOTP Seed";
            this.CheckBoxSeedVisibility.UseVisualStyleBackColor = true;
            this.CheckBoxSeedVisibility.CheckedChanged += new System.EventHandler(this.CheckBoxSeedVisibility_CheckedChanged);
            // 
            // IntervalSecondsSetupLabel
            // 
            this.IntervalSecondsSetupLabel.Location = new System.Drawing.Point(160, 26);
            this.IntervalSecondsSetupLabel.Name = "IntervalSecondsSetupLabel";
            this.IntervalSecondsSetupLabel.Size = new System.Drawing.Size(76, 21);
            this.IntervalSecondsSetupLabel.TabIndex = 1;
            this.IntervalSecondsSetupLabel.TabStop = true;
            this.IntervalSecondsSetupLabel.Text = "seconds";
            this.IntervalSecondsSetupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NumericIntervalSetup
            // 
            this.NumericIntervalSetup.Location = new System.Drawing.Point(82, 26);
            this.NumericIntervalSetup.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.NumericIntervalSetup.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericIntervalSetup.Name = "NumericIntervalSetup";
            this.NumericIntervalSetup.Size = new System.Drawing.Size(72, 20);
            this.NumericIntervalSetup.TabIndex = 0;
            this.NumericIntervalSetup.Tag = "";
            this.NumericIntervalSetup.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // IntervalSetupLabel
            // 
            this.IntervalSetupLabel.Location = new System.Drawing.Point(8, 26);
            this.IntervalSetupLabel.Name = "IntervalSetupLabel";
            this.IntervalSetupLabel.Size = new System.Drawing.Size(50, 21);
            this.IntervalSetupLabel.TabIndex = 2;
            this.IntervalSetupLabel.TabStop = true;
            this.IntervalSetupLabel.Text = "Interval :";
            this.IntervalSetupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // IntervalDescriptionSetupLabel
            // 
            this.IntervalDescriptionSetupLabel.Location = new System.Drawing.Point(5, 5);
            this.IntervalDescriptionSetupLabel.Name = "IntervalDescriptionSetupLabel";
            this.IntervalDescriptionSetupLabel.Size = new System.Drawing.Size(322, 18);
            this.IntervalDescriptionSetupLabel.TabIndex = 0;
            this.IntervalDescriptionSetupLabel.TabStop = true;
            this.IntervalDescriptionSetupLabel.Text = "Please specify the time interval. Between 1 and 180 seconds.";
            // 
            // IntervalPanel
            // 
            this.IntervalPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IntervalPanel.Controls.Add(this.IntervalDescriptionSetupLabel);
            this.IntervalPanel.Controls.Add(this.IntervalSecondsSetupLabel);
            this.IntervalPanel.Controls.Add(this.IntervalSetupLabel);
            this.IntervalPanel.Controls.Add(this.NumericIntervalSetup);
            this.IntervalPanel.Location = new System.Drawing.Point(10, 203);
            this.IntervalPanel.Name = "IntervalPanel";
            this.IntervalPanel.Size = new System.Drawing.Size(345, 60);
            this.IntervalPanel.TabIndex = 1;
            this.IntervalPanel.TabStop = true;
            // 
            // RadioButtonLength8Setup
            // 
            this.RadioButtonLength8Setup.AutoSize = true;
            this.RadioButtonLength8Setup.Location = new System.Drawing.Point(167, 28);
            this.RadioButtonLength8Setup.Name = "RadioButtonLength8Setup";
            this.RadioButtonLength8Setup.Size = new System.Drawing.Size(31, 17);
            this.RadioButtonLength8Setup.TabIndex = 2;
            this.RadioButtonLength8Setup.Tag = "";
            this.RadioButtonLength8Setup.Text = "&8";
            this.RadioButtonLength8Setup.UseVisualStyleBackColor = true;
            // 
            // RadioButtonLength7Setup
            // 
            this.RadioButtonLength7Setup.AutoSize = true;
            this.RadioButtonLength7Setup.Location = new System.Drawing.Point(130, 28);
            this.RadioButtonLength7Setup.Name = "RadioButtonLength7Setup";
            this.RadioButtonLength7Setup.Size = new System.Drawing.Size(31, 17);
            this.RadioButtonLength7Setup.TabIndex = 1;
            this.RadioButtonLength7Setup.Text = "&7";
            this.RadioButtonLength7Setup.UseVisualStyleBackColor = true;
            // 
            // RadioButtonLength6Setup
            // 
            this.RadioButtonLength6Setup.AutoSize = true;
            this.RadioButtonLength6Setup.Checked = true;
            this.RadioButtonLength6Setup.Location = new System.Drawing.Point(93, 28);
            this.RadioButtonLength6Setup.Name = "RadioButtonLength6Setup";
            this.RadioButtonLength6Setup.Size = new System.Drawing.Size(31, 17);
            this.RadioButtonLength6Setup.TabIndex = 0;
            this.RadioButtonLength6Setup.TabStop = true;
            this.RadioButtonLength6Setup.Text = "&6";
            this.RadioButtonLength6Setup.UseVisualStyleBackColor = true;
            // 
            // FormatSetupLabel
            // 
            this.FormatSetupLabel.Location = new System.Drawing.Point(8, 26);
            this.FormatSetupLabel.Name = "FormatSetupLabel";
            this.FormatSetupLabel.Size = new System.Drawing.Size(79, 21);
            this.FormatSetupLabel.TabIndex = 5;
            this.FormatSetupLabel.TabStop = true;
            this.FormatSetupLabel.Text = "TOTP Format :";
            this.FormatSetupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormatDescriptionSetupLabel
            // 
            this.FormatDescriptionSetupLabel.Location = new System.Drawing.Point(5, 5);
            this.FormatDescriptionSetupLabel.Name = "FormatDescriptionSetupLabel";
            this.FormatDescriptionSetupLabel.Size = new System.Drawing.Size(322, 20);
            this.FormatDescriptionSetupLabel.TabIndex = 1;
            this.FormatDescriptionSetupLabel.TabStop = true;
            this.FormatDescriptionSetupLabel.Text = "Please select the TOTP Format. (6,7,8 digits or Steam)";
            // 
            // LengthPanel
            // 
            this.LengthPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LengthPanel.Controls.Add(this.RadioButtonSteamFormatSetup);
            this.LengthPanel.Controls.Add(this.FormatDescriptionSetupLabel);
            this.LengthPanel.Controls.Add(this.FormatSetupLabel);
            this.LengthPanel.Controls.Add(this.RadioButtonLength8Setup);
            this.LengthPanel.Controls.Add(this.RadioButtonLength7Setup);
            this.LengthPanel.Controls.Add(this.RadioButtonLength6Setup);
            this.LengthPanel.Location = new System.Drawing.Point(10, 270);
            this.LengthPanel.Name = "LengthPanel";
            this.LengthPanel.Size = new System.Drawing.Size(345, 58);
            this.LengthPanel.TabIndex = 2;
            this.LengthPanel.TabStop = true;
            // 
            // RadioButtonSteamFormatSetup
            // 
            this.RadioButtonSteamFormatSetup.AutoSize = true;
            this.RadioButtonSteamFormatSetup.Location = new System.Drawing.Point(204, 28);
            this.RadioButtonSteamFormatSetup.Name = "RadioButtonSteamFormatSetup";
            this.RadioButtonSteamFormatSetup.Size = new System.Drawing.Size(55, 17);
            this.RadioButtonSteamFormatSetup.TabIndex = 3;
            this.RadioButtonSteamFormatSetup.Tag = "";
            this.RadioButtonSteamFormatSetup.Text = "&Steam";
            this.RadioButtonSteamFormatSetup.UseVisualStyleBackColor = true;
            // 
            // ComboBoxTimeCorrectionSetup
            // 
            this.ComboBoxTimeCorrectionSetup.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.ComboBoxTimeCorrectionSetup.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboBoxTimeCorrectionSetup.FormattingEnabled = true;
            this.ComboBoxTimeCorrectionSetup.Location = new System.Drawing.Point(82, 25);
            this.ComboBoxTimeCorrectionSetup.Name = "ComboBoxTimeCorrectionSetup";
            this.ComboBoxTimeCorrectionSetup.Size = new System.Drawing.Size(200, 21);
            this.ComboBoxTimeCorrectionSetup.TabIndex = 0;
            this.ComboBoxTimeCorrectionSetup.Tag = "";
            // 
            // TimeCorrectionSetupLabel
            // 
            this.TimeCorrectionSetupLabel.Location = new System.Drawing.Point(5, 26);
            this.TimeCorrectionSetupLabel.Name = "TimeCorrectionSetupLabel";
            this.TimeCorrectionSetupLabel.Size = new System.Drawing.Size(71, 20);
            this.TimeCorrectionSetupLabel.TabIndex = 1;
            this.TimeCorrectionSetupLabel.TabStop = true;
            this.TimeCorrectionSetupLabel.Text = "Server URL :";
            this.TimeCorrectionSetupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TimeCorrectionDescriptionSetupLabel
            // 
            this.TimeCorrectionDescriptionSetupLabel.Location = new System.Drawing.Point(5, 5);
            this.TimeCorrectionDescriptionSetupLabel.Name = "TimeCorrectionDescriptionSetupLabel";
            this.TimeCorrectionDescriptionSetupLabel.Size = new System.Drawing.Size(322, 20);
            this.TimeCorrectionDescriptionSetupLabel.TabIndex = 0;
            this.TimeCorrectionDescriptionSetupLabel.TabStop = true;
            this.TimeCorrectionDescriptionSetupLabel.Text = "Time correction is optional but strongly recommended.\r\n";
            // 
            // TimeCorrectionPanel
            // 
            this.TimeCorrectionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TimeCorrectionPanel.Controls.Add(this.TimeCorrectionDescriptionSetupLabel);
            this.TimeCorrectionPanel.Controls.Add(this.ComboBoxTimeCorrectionSetup);
            this.TimeCorrectionPanel.Controls.Add(this.TimeCorrectionSetupLabel);
            this.TimeCorrectionPanel.Location = new System.Drawing.Point(10, 335);
            this.TimeCorrectionPanel.Name = "TimeCorrectionPanel";
            this.TimeCorrectionPanel.Size = new System.Drawing.Size(345, 60);
            this.TimeCorrectionPanel.TabIndex = 3;
            this.TimeCorrectionPanel.TabStop = true;
            // 
            // CancelSetupButton
            // 
            this.CancelSetupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CancelSetupButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelSetupButton.Location = new System.Drawing.Point(10, 410);
            this.CancelSetupButton.Name = "CancelSetupButton";
            this.CancelSetupButton.Size = new System.Drawing.Size(75, 23);
            this.CancelSetupButton.TabIndex = 4;
            this.CancelSetupButton.Text = "&Cancel";
            this.CancelSetupButton.UseVisualStyleBackColor = true;
            // 
            // DeleteSetupButton
            // 
            this.DeleteSetupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteSetupButton.Location = new System.Drawing.Point(191, 410);
            this.DeleteSetupButton.Name = "DeleteSetupButton";
            this.HelpProviderSetup.SetShowHelp(this.DeleteSetupButton, true);
            this.DeleteSetupButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteSetupButton.TabIndex = 6;
            this.DeleteSetupButton.Tag = "";
            this.DeleteSetupButton.Text = "&Delete";
            this.DeleteSetupButton.UseVisualStyleBackColor = true;
            this.DeleteSetupButton.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // FinishSetupButton
            // 
            this.FinishSetupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpProviderSetup.SetHelpString(this.FinishSetupButton, "");
            this.FinishSetupButton.Location = new System.Drawing.Point(280, 410);
            this.FinishSetupButton.Name = "FinishSetupButton";
            this.HelpProviderSetup.SetShowHelp(this.FinishSetupButton, true);
            this.FinishSetupButton.Size = new System.Drawing.Size(75, 23);
            this.FinishSetupButton.TabIndex = 5;
            this.FinishSetupButton.Tag = "";
            this.FinishSetupButton.Text = "&Finish";
            this.FinishSetupButton.UseVisualStyleBackColor = true;
            this.FinishSetupButton.Click += new System.EventHandler(this.ButtonFinish_Click);
            // 
            // ErrorProviderSetup
            // 
            this.ErrorProviderSetup.ContainerControl = this;
            // 
            // SetupTOTP
            // 
            this.AcceptButton = this.FinishSetupButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelSetupButton;
            this.ClientSize = new System.Drawing.Size(365, 445);
            this.Controls.Add(this.DeleteSetupButton);
            this.Controls.Add(this.FinishSetupButton);
            this.Controls.Add(this.CancelSetupButton);
            this.Controls.Add(this.TimeCorrectionPanel);
            this.Controls.Add(this.LengthPanel);
            this.Controls.Add(this.IntervalPanel);
            this.Controls.Add(this.SeedPanel);
            this.Controls.Add(this.InfoPanel);
            this.Controls.Add(this.PictureBoxAbout);
            this.Controls.Add(this.TitleAboutLabel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupTOTP";
            this.ShowInTaskbar = false;
            this.Text = "SetupTOTP";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SetupTOTP_FormClosed);
            this.Load += new System.EventHandler(this.SetupTOTP_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbout)).EndInit();
            this.InfoPanel.ResumeLayout(false);
            this.SeedPanel.ResumeLayout(false);
            this.SeedPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericIntervalSetup)).EndInit();
            this.IntervalPanel.ResumeLayout(false);
            this.LengthPanel.ResumeLayout(false);
            this.LengthPanel.PerformLayout();
            this.TimeCorrectionPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProviderSetup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label TitleAboutLabel;
        private System.Windows.Forms.PictureBox PictureBoxAbout;
        private System.Windows.Forms.TextBox TextBoxSeedSetup;
        private System.Windows.Forms.Label SeedDescriptionSetupLabel;
        private System.Windows.Forms.Panel InfoPanel;
        private System.Windows.Forms.Label StartSetupLabel;
        private System.Windows.Forms.Panel SeedPanel;
        private System.Windows.Forms.Label SeedSetupLabel;
        private System.Windows.Forms.CheckBox CheckBoxSeedVisibility;
        private System.Windows.Forms.Label IntervalSecondsSetupLabel;
        private System.Windows.Forms.NumericUpDown NumericIntervalSetup;
        private System.Windows.Forms.Label IntervalSetupLabel;
        private System.Windows.Forms.Label IntervalDescriptionSetupLabel;
        private System.Windows.Forms.Panel IntervalPanel;
        private System.Windows.Forms.RadioButton RadioButtonLength8Setup;
        private System.Windows.Forms.RadioButton RadioButtonLength7Setup;
        private System.Windows.Forms.RadioButton RadioButtonLength6Setup;
        private System.Windows.Forms.Label FormatSetupLabel;
        private System.Windows.Forms.Label FormatDescriptionSetupLabel;
        private System.Windows.Forms.Panel LengthPanel;
        private System.Windows.Forms.RadioButton RadioButtonSteamFormatSetup;
        private System.Windows.Forms.ComboBox ComboBoxTimeCorrectionSetup;
        private System.Windows.Forms.Label TimeCorrectionSetupLabel;
        private System.Windows.Forms.Label TimeCorrectionDescriptionSetupLabel;
        private System.Windows.Forms.Panel TimeCorrectionPanel;
        private System.Windows.Forms.Button CancelSetupButton;
        private System.Windows.Forms.HelpProvider HelpProviderSetup;
        private System.Windows.Forms.ErrorProvider ErrorProviderSetup;
        private System.Windows.Forms.Button DeleteSetupButton;
        private System.Windows.Forms.Button FinishSetupButton;
    }
}