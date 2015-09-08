namespace KeeTrayTOTP
{
    partial class FormSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetup));
            this.ButtonNext = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonBack = new System.Windows.Forms.Button();
            this.PictureBoxAbout = new System.Windows.Forms.PictureBox();
            this.LabelBannerAbout = new System.Windows.Forms.Label();
            this.LabelDescriptionIntervalSetup = new System.Windows.Forms.Label();
            this.LabelIntervalSetup = new System.Windows.Forms.Label();
            this.HelpProviderSetup = new System.Windows.Forms.HelpProvider();
            this.NumericIntervalSetup = new System.Windows.Forms.NumericUpDown();
            this.CheckBoxSeedVisibility = new System.Windows.Forms.CheckBox();
            this.RadioButtonLength8Setup = new System.Windows.Forms.RadioButton();
            this.RadioButtonLength6Setup = new System.Windows.Forms.RadioButton();
            this.TextBoxSeedSetup = new System.Windows.Forms.TextBox();
            this.ComboBoxTimeCorrectionSetup = new System.Windows.Forms.ComboBox();
            this.PanelIntervalSetup = new System.Windows.Forms.Panel();
            this.LabelSecondsSetup = new System.Windows.Forms.Label();
            this.LabelTitleAbout = new System.Windows.Forms.Label();
            this.PanelLengthSetup = new System.Windows.Forms.Panel();
            this.LabelLengthSetup = new System.Windows.Forms.Label();
            this.LabelDescriptionLength = new System.Windows.Forms.Label();
            this.PanelSeedSetup = new System.Windows.Forms.Panel();
            this.LabelSeedSetup = new System.Windows.Forms.Label();
            this.LabelDescriptionSeedSetup = new System.Windows.Forms.Label();
            this.PanelFinishSetup = new System.Windows.Forms.Panel();
            this.LabelFinishSetup = new System.Windows.Forms.Label();
            this.PanelStartSetup = new System.Windows.Forms.Panel();
            this.LabelStartSetup = new System.Windows.Forms.Label();
            this.PanelTimeCorrectionSetup = new System.Windows.Forms.Panel();
            this.LabelTimeCorrectionSetup = new System.Windows.Forms.Label();
            this.LabelDescriptionTimeCorrectionSetup = new System.Windows.Forms.Label();
            this.PanelReadySetup = new System.Windows.Forms.Panel();
            this.LabelReady = new System.Windows.Forms.Label();
            this.ErrorProviderSetup = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericIntervalSetup)).BeginInit();
            this.PanelIntervalSetup.SuspendLayout();
            this.PanelLengthSetup.SuspendLayout();
            this.PanelSeedSetup.SuspendLayout();
            this.PanelFinishSetup.SuspendLayout();
            this.PanelStartSetup.SuspendLayout();
            this.PanelTimeCorrectionSetup.SuspendLayout();
            this.PanelReadySetup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProviderSetup)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonNext
            // 
            this.ButtonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpProviderSetup.SetHelpString(this.ButtonNext, "");
            this.ButtonNext.Location = new System.Drawing.Point(312, 209);
            this.ButtonNext.Name = "ButtonNext";
            this.HelpProviderSetup.SetShowHelp(this.ButtonNext, true);
            this.ButtonNext.Size = new System.Drawing.Size(75, 23);
            this.ButtonNext.TabIndex = 1;
            this.ButtonNext.Tag = "";
            this.ButtonNext.Text = "&Next >";
            this.ButtonNext.UseVisualStyleBackColor = true;
            this.ButtonNext.Click += new System.EventHandler(this.ButtonNext_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.HelpProviderSetup.SetHelpString(this.ButtonCancel, "Cancels the wizard.");
            this.ButtonCancel.Location = new System.Drawing.Point(17, 209);
            this.ButtonCancel.Name = "ButtonCancel";
            this.HelpProviderSetup.SetShowHelp(this.ButtonCancel, true);
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 3;
            this.ButtonCancel.Text = "&Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonBack
            // 
            this.ButtonBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonBack.Location = new System.Drawing.Point(223, 209);
            this.ButtonBack.Name = "ButtonBack";
            this.HelpProviderSetup.SetShowHelp(this.ButtonBack, true);
            this.ButtonBack.Size = new System.Drawing.Size(75, 23);
            this.ButtonBack.TabIndex = 2;
            this.ButtonBack.Tag = "";
            this.ButtonBack.Text = "< &Back";
            this.ButtonBack.UseVisualStyleBackColor = true;
            this.ButtonBack.Click += new System.EventHandler(this.ButtonBack_Click);
            // 
            // PictureBoxAbout
            // 
            this.PictureBoxAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.PictureBoxAbout.Image = ((System.Drawing.Image)(resources.GetObject("PictureBoxAbout.Image")));
            this.PictureBoxAbout.Location = new System.Drawing.Point(348, 3);
            this.PictureBoxAbout.Name = "PictureBoxAbout";
            this.PictureBoxAbout.Size = new System.Drawing.Size(54, 54);
            this.PictureBoxAbout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PictureBoxAbout.TabIndex = 7;
            this.PictureBoxAbout.TabStop = false;
            // 
            // LabelBannerAbout
            // 
            this.LabelBannerAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.LabelBannerAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelBannerAbout.Location = new System.Drawing.Point(0, 0);
            this.LabelBannerAbout.Name = "LabelBannerAbout";
            this.LabelBannerAbout.Size = new System.Drawing.Size(404, 60);
            this.LabelBannerAbout.TabIndex = 5;
            // 
            // LabelDescriptionIntervalSetup
            // 
            this.LabelDescriptionIntervalSetup.Location = new System.Drawing.Point(13, 13);
            this.LabelDescriptionIntervalSetup.Name = "LabelDescriptionIntervalSetup";
            this.LabelDescriptionIntervalSetup.Size = new System.Drawing.Size(355, 115);
            this.LabelDescriptionIntervalSetup.TabIndex = 0;
            this.LabelDescriptionIntervalSetup.Text = "Please specify the time interval.\r\n\r\nValid values are included in the range of 1 " +
    "and 60.";
            // 
            // LabelIntervalSetup
            // 
            this.LabelIntervalSetup.Location = new System.Drawing.Point(54, 86);
            this.LabelIntervalSetup.Name = "LabelIntervalSetup";
            this.LabelIntervalSetup.Size = new System.Drawing.Size(93, 21);
            this.LabelIntervalSetup.TabIndex = 1;
            this.LabelIntervalSetup.Text = "Interval :";
            this.LabelIntervalSetup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NumericIntervalSetup
            // 
            this.HelpProviderSetup.SetHelpString(this.NumericIntervalSetup, "Interval defining the duration of each generated TOTP.");
            this.NumericIntervalSetup.Location = new System.Drawing.Point(165, 86);
            this.NumericIntervalSetup.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.NumericIntervalSetup.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericIntervalSetup.Name = "NumericIntervalSetup";
            this.HelpProviderSetup.SetShowHelp(this.NumericIntervalSetup, true);
            this.NumericIntervalSetup.Size = new System.Drawing.Size(72, 21);
            this.NumericIntervalSetup.TabIndex = 2;
            this.NumericIntervalSetup.Tag = "";
            this.NumericIntervalSetup.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // CheckBoxSeedVisibility
            // 
            this.CheckBoxSeedVisibility.AutoSize = true;
            this.HelpProviderSetup.SetHelpString(this.CheckBoxSeedVisibility, "Toggle\'s the display of the seed (hidden or visible).");
            this.CheckBoxSeedVisibility.Location = new System.Drawing.Point(173, 114);
            this.CheckBoxSeedVisibility.Name = "CheckBoxSeedVisibility";
            this.HelpProviderSetup.SetShowHelp(this.CheckBoxSeedVisibility, true);
            this.CheckBoxSeedVisibility.Size = new System.Drawing.Size(108, 17);
            this.CheckBoxSeedVisibility.TabIndex = 3;
            this.CheckBoxSeedVisibility.Tag = "";
            this.CheckBoxSeedVisibility.Text = "Show TOTP Seed";
            this.CheckBoxSeedVisibility.UseVisualStyleBackColor = true;
            this.CheckBoxSeedVisibility.CheckedChanged += new System.EventHandler(this.CheckBoxSeedVisibility_CheckedChanged);
            // 
            // RadioButtonLength8Setup
            // 
            this.RadioButtonLength8Setup.AutoSize = true;
            this.HelpProviderSetup.SetHelpString(this.RadioButtonLength8Setup, "TOTP Length of 8 numbers.");
            this.RadioButtonLength8Setup.Location = new System.Drawing.Point(267, 90);
            this.RadioButtonLength8Setup.Name = "RadioButtonLength8Setup";
            this.HelpProviderSetup.SetShowHelp(this.RadioButtonLength8Setup, true);
            this.RadioButtonLength8Setup.Size = new System.Drawing.Size(31, 17);
            this.RadioButtonLength8Setup.TabIndex = 3;
            this.RadioButtonLength8Setup.Tag = "";
            this.RadioButtonLength8Setup.Text = "&8";
            this.RadioButtonLength8Setup.UseVisualStyleBackColor = true;
            // 
            // RadioButtonLength6Setup
            // 
            this.RadioButtonLength6Setup.AutoSize = true;
            this.HelpProviderSetup.SetHelpString(this.RadioButtonLength6Setup, "TOTP Length of 6 numbers.");
            this.RadioButtonLength6Setup.Location = new System.Drawing.Point(202, 90);
            this.RadioButtonLength6Setup.Name = "RadioButtonLength6Setup";
            this.HelpProviderSetup.SetShowHelp(this.RadioButtonLength6Setup, true);
            this.RadioButtonLength6Setup.Size = new System.Drawing.Size(31, 17);
            this.RadioButtonLength6Setup.TabIndex = 2;
            this.RadioButtonLength6Setup.Text = "&6";
            this.RadioButtonLength6Setup.UseVisualStyleBackColor = true;
            // 
            // TextBoxSeedSetup
            // 
            this.TextBoxSeedSetup.Location = new System.Drawing.Point(144, 88);
            this.TextBoxSeedSetup.MaxLength = 255;
            this.TextBoxSeedSetup.Name = "TextBoxSeedSetup";
            this.HelpProviderSetup.SetShowHelp(this.TextBoxSeedSetup, true);
            this.TextBoxSeedSetup.Size = new System.Drawing.Size(179, 21);
            this.TextBoxSeedSetup.TabIndex = 2;
            this.TextBoxSeedSetup.Tag = "";
            this.TextBoxSeedSetup.UseSystemPasswordChar = true;
            // 
            // ComboBoxTimeCorrectionSetup
            // 
            this.ComboBoxTimeCorrectionSetup.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.ComboBoxTimeCorrectionSetup.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboBoxTimeCorrectionSetup.FormattingEnabled = true;
            this.ComboBoxTimeCorrectionSetup.Location = new System.Drawing.Point(144, 88);
            this.ComboBoxTimeCorrectionSetup.Name = "ComboBoxTimeCorrectionSetup";
            this.HelpProviderSetup.SetShowHelp(this.ComboBoxTimeCorrectionSetup, true);
            this.ComboBoxTimeCorrectionSetup.Size = new System.Drawing.Size(200, 21);
            this.ComboBoxTimeCorrectionSetup.TabIndex = 2;
            this.ComboBoxTimeCorrectionSetup.Tag = "";
            // 
            // PanelIntervalSetup
            // 
            this.PanelIntervalSetup.Controls.Add(this.LabelSecondsSetup);
            this.PanelIntervalSetup.Controls.Add(this.NumericIntervalSetup);
            this.PanelIntervalSetup.Controls.Add(this.LabelIntervalSetup);
            this.PanelIntervalSetup.Controls.Add(this.LabelDescriptionIntervalSetup);
            this.PanelIntervalSetup.Location = new System.Drawing.Point(12, 63);
            this.PanelIntervalSetup.Name = "PanelIntervalSetup";
            this.PanelIntervalSetup.Size = new System.Drawing.Size(380, 140);
            this.PanelIntervalSetup.TabIndex = 6;
            this.PanelIntervalSetup.Visible = false;
            // 
            // LabelSecondsSetup
            // 
            this.LabelSecondsSetup.Location = new System.Drawing.Point(251, 86);
            this.LabelSecondsSetup.Name = "LabelSecondsSetup";
            this.LabelSecondsSetup.Size = new System.Drawing.Size(76, 21);
            this.LabelSecondsSetup.TabIndex = 3;
            this.LabelSecondsSetup.Text = "seconds";
            this.LabelSecondsSetup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelTitleAbout
            // 
            this.LabelTitleAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.LabelTitleAbout.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTitleAbout.Location = new System.Drawing.Point(13, 9);
            this.LabelTitleAbout.Name = "LabelTitleAbout";
            this.LabelTitleAbout.Size = new System.Drawing.Size(329, 44);
            this.LabelTitleAbout.TabIndex = 4;
            this.LabelTitleAbout.Text = "TOTP Setup Wizard";
            this.LabelTitleAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PanelLengthSetup
            // 
            this.PanelLengthSetup.Controls.Add(this.RadioButtonLength8Setup);
            this.PanelLengthSetup.Controls.Add(this.RadioButtonLength6Setup);
            this.PanelLengthSetup.Controls.Add(this.LabelLengthSetup);
            this.PanelLengthSetup.Controls.Add(this.LabelDescriptionLength);
            this.PanelLengthSetup.Location = new System.Drawing.Point(12, 63);
            this.PanelLengthSetup.Name = "PanelLengthSetup";
            this.PanelLengthSetup.Size = new System.Drawing.Size(380, 140);
            this.PanelLengthSetup.TabIndex = 8;
            this.PanelLengthSetup.Visible = false;
            // 
            // LabelLengthSetup
            // 
            this.LabelLengthSetup.Location = new System.Drawing.Point(68, 87);
            this.LabelLengthSetup.Name = "LabelLengthSetup";
            this.LabelLengthSetup.Size = new System.Drawing.Size(100, 21);
            this.LabelLengthSetup.TabIndex = 1;
            this.LabelLengthSetup.Text = "TOTP Length :";
            this.LabelLengthSetup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LabelDescriptionLength
            // 
            this.LabelDescriptionLength.Location = new System.Drawing.Point(13, 13);
            this.LabelDescriptionLength.Name = "LabelDescriptionLength";
            this.LabelDescriptionLength.Size = new System.Drawing.Size(355, 115);
            this.LabelDescriptionLength.TabIndex = 0;
            this.LabelDescriptionLength.Text = "Please select the TOTP length.\r\n\r\nTOTP are usually 6 numbers but this plugin supp" +
    "ort 8 numbers as well.";
            // 
            // PanelSeedSetup
            // 
            this.PanelSeedSetup.Controls.Add(this.LabelSeedSetup);
            this.PanelSeedSetup.Controls.Add(this.TextBoxSeedSetup);
            this.PanelSeedSetup.Controls.Add(this.CheckBoxSeedVisibility);
            this.PanelSeedSetup.Controls.Add(this.LabelDescriptionSeedSetup);
            this.PanelSeedSetup.Location = new System.Drawing.Point(12, 63);
            this.PanelSeedSetup.Name = "PanelSeedSetup";
            this.PanelSeedSetup.Size = new System.Drawing.Size(380, 140);
            this.PanelSeedSetup.TabIndex = 6;
            this.PanelSeedSetup.Visible = false;
            // 
            // LabelSeedSetup
            // 
            this.LabelSeedSetup.Location = new System.Drawing.Point(29, 88);
            this.LabelSeedSetup.Name = "LabelSeedSetup";
            this.LabelSeedSetup.Size = new System.Drawing.Size(100, 21);
            this.LabelSeedSetup.TabIndex = 1;
            this.LabelSeedSetup.Text = "TOTP Seed :";
            this.LabelSeedSetup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LabelDescriptionSeedSetup
            // 
            this.LabelDescriptionSeedSetup.Location = new System.Drawing.Point(13, 13);
            this.LabelDescriptionSeedSetup.Name = "LabelDescriptionSeedSetup";
            this.LabelDescriptionSeedSetup.Size = new System.Drawing.Size(355, 115);
            this.LabelDescriptionSeedSetup.TabIndex = 0;
            this.LabelDescriptionSeedSetup.Text = "Please input the TOTP seed.\r\n\r\nThe only format currently supported is Base32. Spa" +
    "ces are trimmed automatically. The box to the right will reveal the field\'s cont" +
    "ent.";
            // 
            // PanelFinishSetup
            // 
            this.PanelFinishSetup.Controls.Add(this.LabelFinishSetup);
            this.PanelFinishSetup.Location = new System.Drawing.Point(12, 63);
            this.PanelFinishSetup.Name = "PanelFinishSetup";
            this.PanelFinishSetup.Size = new System.Drawing.Size(380, 140);
            this.PanelFinishSetup.TabIndex = 11;
            this.PanelFinishSetup.Visible = false;
            // 
            // LabelFinishSetup
            // 
            this.LabelFinishSetup.Location = new System.Drawing.Point(13, 13);
            this.LabelFinishSetup.Name = "LabelFinishSetup";
            this.LabelFinishSetup.Size = new System.Drawing.Size(355, 115);
            this.LabelFinishSetup.TabIndex = 0;
            this.LabelFinishSetup.Text = "Creation process succeeded!\r\n\r\nYou may know use the TOTP to access your account.\r" +
    "\n\r\nClick \"Finish\" to close this window.";
            // 
            // PanelStartSetup
            // 
            this.PanelStartSetup.Controls.Add(this.LabelStartSetup);
            this.PanelStartSetup.Location = new System.Drawing.Point(12, 63);
            this.PanelStartSetup.Name = "PanelStartSetup";
            this.PanelStartSetup.Size = new System.Drawing.Size(380, 140);
            this.PanelStartSetup.TabIndex = 0;
            this.PanelStartSetup.Visible = false;
            // 
            // LabelStartSetup
            // 
            this.LabelStartSetup.Location = new System.Drawing.Point(13, 13);
            this.LabelStartSetup.Name = "LabelStartSetup";
            this.LabelStartSetup.Size = new System.Drawing.Size(355, 115);
            this.LabelStartSetup.TabIndex = 0;
            this.LabelStartSetup.Text = resources.GetString("LabelStartSetup.Text");
            // 
            // PanelTimeCorrectionSetup
            // 
            this.PanelTimeCorrectionSetup.Controls.Add(this.ComboBoxTimeCorrectionSetup);
            this.PanelTimeCorrectionSetup.Controls.Add(this.LabelTimeCorrectionSetup);
            this.PanelTimeCorrectionSetup.Controls.Add(this.LabelDescriptionTimeCorrectionSetup);
            this.PanelTimeCorrectionSetup.Location = new System.Drawing.Point(12, 63);
            this.PanelTimeCorrectionSetup.Name = "PanelTimeCorrectionSetup";
            this.PanelTimeCorrectionSetup.Size = new System.Drawing.Size(380, 140);
            this.PanelTimeCorrectionSetup.TabIndex = 10;
            this.PanelTimeCorrectionSetup.Visible = false;
            // 
            // LabelTimeCorrectionSetup
            // 
            this.LabelTimeCorrectionSetup.Location = new System.Drawing.Point(29, 88);
            this.LabelTimeCorrectionSetup.Name = "LabelTimeCorrectionSetup";
            this.LabelTimeCorrectionSetup.Size = new System.Drawing.Size(100, 21);
            this.LabelTimeCorrectionSetup.TabIndex = 1;
            this.LabelTimeCorrectionSetup.Text = "Server URL :";
            this.LabelTimeCorrectionSetup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LabelDescriptionTimeCorrectionSetup
            // 
            this.LabelDescriptionTimeCorrectionSetup.Location = new System.Drawing.Point(13, 13);
            this.LabelDescriptionTimeCorrectionSetup.Name = "LabelDescriptionTimeCorrectionSetup";
            this.LabelDescriptionTimeCorrectionSetup.Size = new System.Drawing.Size(355, 115);
            this.LabelDescriptionTimeCorrectionSetup.TabIndex = 0;
            this.LabelDescriptionTimeCorrectionSetup.Text = "If you wish to use time correction, you may enter the URL of the validation serve" +
    "r.\r\n\r\nThis step is optionnal but strongly recommended.";
            // 
            // PanelReadySetup
            // 
            this.PanelReadySetup.Controls.Add(this.LabelReady);
            this.PanelReadySetup.Location = new System.Drawing.Point(12, 63);
            this.PanelReadySetup.Name = "PanelReadySetup";
            this.PanelReadySetup.Size = new System.Drawing.Size(380, 140);
            this.PanelReadySetup.TabIndex = 9;
            this.PanelReadySetup.Visible = false;
            // 
            // LabelReady
            // 
            this.LabelReady.Location = new System.Drawing.Point(13, 13);
            this.LabelReady.Name = "LabelReady";
            this.LabelReady.Size = new System.Drawing.Size(355, 115);
            this.LabelReady.TabIndex = 0;
            this.LabelReady.Text = "The wizard has all the required information to create the TOTP.\r\n\r\nClick \"Proceed" +
    "\" to create it or apply modifications.";
            // 
            // ErrorProviderSetup
            // 
            this.ErrorProviderSetup.ContainerControl = this;
            // 
            // FormSetup
            // 
            this.AcceptButton = this.ButtonNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(404, 244);
            this.Controls.Add(this.LabelTitleAbout);
            this.Controls.Add(this.PictureBoxAbout);
            this.Controls.Add(this.LabelBannerAbout);
            this.Controls.Add(this.ButtonBack);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonNext);
            this.Controls.Add(this.PanelStartSetup);
            this.Controls.Add(this.PanelIntervalSetup);
            this.Controls.Add(this.PanelLengthSetup);
            this.Controls.Add(this.PanelSeedSetup);
            this.Controls.Add(this.PanelTimeCorrectionSetup);
            this.Controls.Add(this.PanelReadySetup);
            this.Controls.Add(this.PanelFinishSetup);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSetup";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "_form setup_";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSetup_FormClosing);
            this.Load += new System.EventHandler(this.FormSetup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericIntervalSetup)).EndInit();
            this.PanelIntervalSetup.ResumeLayout(false);
            this.PanelLengthSetup.ResumeLayout(false);
            this.PanelLengthSetup.PerformLayout();
            this.PanelSeedSetup.ResumeLayout(false);
            this.PanelSeedSetup.PerformLayout();
            this.PanelFinishSetup.ResumeLayout(false);
            this.PanelStartSetup.ResumeLayout(false);
            this.PanelTimeCorrectionSetup.ResumeLayout(false);
            this.PanelReadySetup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProviderSetup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonNext;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonBack;
        private System.Windows.Forms.PictureBox PictureBoxAbout;
        private System.Windows.Forms.Label LabelBannerAbout;
        private System.Windows.Forms.Label LabelDescriptionIntervalSetup;
        private System.Windows.Forms.Label LabelIntervalSetup;
        private System.Windows.Forms.HelpProvider HelpProviderSetup;
        private System.Windows.Forms.Panel PanelIntervalSetup;
        private System.Windows.Forms.Label LabelTitleAbout;
        private System.Windows.Forms.Panel PanelLengthSetup;
        private System.Windows.Forms.Label LabelDescriptionLength;
        private System.Windows.Forms.Label LabelLengthSetup;
        private System.Windows.Forms.Panel PanelSeedSetup;
        private System.Windows.Forms.Label LabelDescriptionSeedSetup;
        private System.Windows.Forms.TextBox TextBoxSeedSetup;
        private System.Windows.Forms.Label LabelSeedSetup;
        private System.Windows.Forms.NumericUpDown NumericIntervalSetup;
        private System.Windows.Forms.RadioButton RadioButtonLength8Setup;
        private System.Windows.Forms.RadioButton RadioButtonLength6Setup;
        private System.Windows.Forms.Panel PanelFinishSetup;
        private System.Windows.Forms.Label LabelFinishSetup;
        private System.Windows.Forms.Panel PanelStartSetup;
        private System.Windows.Forms.Label LabelStartSetup;
        private System.Windows.Forms.Panel PanelTimeCorrectionSetup;
        private System.Windows.Forms.Label LabelDescriptionTimeCorrectionSetup;
        private System.Windows.Forms.Label LabelTimeCorrectionSetup;
        private System.Windows.Forms.Panel PanelReadySetup;
        private System.Windows.Forms.Label LabelReady;
        private System.Windows.Forms.ErrorProvider ErrorProviderSetup;
        private System.Windows.Forms.Label LabelSecondsSetup;
        private System.Windows.Forms.ComboBox ComboBoxTimeCorrectionSetup;
        private System.Windows.Forms.CheckBox CheckBoxSeedVisibility;
    }
}