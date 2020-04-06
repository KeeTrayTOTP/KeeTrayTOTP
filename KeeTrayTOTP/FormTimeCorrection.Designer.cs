namespace KeeTrayTOTP
{
    partial class FormTimeCorrection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTimeCorrection));
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.LabelServerHttpUrlTimeCorrection = new System.Windows.Forms.Label();
            this.WorkerWaitForCheck = new System.ComponentModel.BackgroundWorker();
            this.ImageListErrorProvider = new System.Windows.Forms.ImageList(this.components);
            this.LabelStatusDescriptionTimeCorrection = new System.Windows.Forms.Label();
            this.PictureBoxTimeCorrection = new System.Windows.Forms.PictureBox();
            this.LabelStatusTimeCorrection = new System.Windows.Forms.Label();
            this.ButtonVerify = new System.Windows.Forms.Button();
            this.LabelTitleAbout = new System.Windows.Forms.Label();
            this.PictureBoxAbout = new System.Windows.Forms.PictureBox();
            this.LabelBannerAbout = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ErrorProviderTimeCorrection = new System.Windows.Forms.ErrorProvider(this.components);
            this.ComboBoxUrlTimeCorrection = new System.Windows.Forms.ComboBox();
            this.HelpProviderTimeCorrection = new System.Windows.Forms.HelpProvider();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxTimeCorrection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProviderTimeCorrection)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Enabled = false;
            this.HelpProviderTimeCorrection.SetHelpString(this.ButtonOK, "Adds the server to the time correction list.");
            this.ButtonOK.Location = new System.Drawing.Point(211, 194);
            this.ButtonOK.Name = "ButtonOK";
            this.HelpProviderTimeCorrection.SetShowHelp(this.ButtonOK, true);
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 0;
            this.ButtonOK.Text = "&OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.HelpProviderTimeCorrection.SetHelpString(this.ButtonCancel, "Cancels the process and closes the window.");
            this.ButtonCancel.Location = new System.Drawing.Point(311, 194);
            this.ButtonCancel.Name = "ButtonCancel";
            this.HelpProviderTimeCorrection.SetShowHelp(this.ButtonCancel, true);
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 6;
            this.ButtonCancel.Text = "&Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // LabelServerHttpUrlTimeCorrection
            // 
            this.LabelServerHttpUrlTimeCorrection.Location = new System.Drawing.Point(35, 119);
            this.LabelServerHttpUrlTimeCorrection.Name = "LabelServerHttpUrlTimeCorrection";
            this.LabelServerHttpUrlTimeCorrection.Size = new System.Drawing.Size(96, 21);
            this.LabelServerHttpUrlTimeCorrection.TabIndex = 1;
            this.LabelServerHttpUrlTimeCorrection.Text = "Server URL :";
            this.LabelServerHttpUrlTimeCorrection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // WorkerWaitForCheck
            // 
            this.WorkerWaitForCheck.WorkerSupportsCancellation = true;
            this.WorkerWaitForCheck.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WorkerWaitForCheck_DoWork);
            this.WorkerWaitForCheck.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WorkerWaitForCheck_RunWorkerCompleted);
            // 
            // ImageListErrorProvider
            // 
            this.ImageListErrorProvider.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageListErrorProvider.ImageStream")));
            this.ImageListErrorProvider.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageListErrorProvider.Images.SetKeyName(0, "accept.png");
            this.ImageListErrorProvider.Images.SetKeyName(1, "hourglass.png");
            this.ImageListErrorProvider.Images.SetKeyName(2, "error.png");
            this.ImageListErrorProvider.Images.SetKeyName(3, "cancel.png");
            // 
            // LabelStatusDescriptionTimeCorrection
            // 
            this.LabelStatusDescriptionTimeCorrection.Location = new System.Drawing.Point(38, 153);
            this.LabelStatusDescriptionTimeCorrection.Name = "LabelStatusDescriptionTimeCorrection";
            this.LabelStatusDescriptionTimeCorrection.Size = new System.Drawing.Size(93, 21);
            this.LabelStatusDescriptionTimeCorrection.TabIndex = 3;
            this.LabelStatusDescriptionTimeCorrection.Text = "Server status :";
            this.LabelStatusDescriptionTimeCorrection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PictureBoxTimeCorrection
            // 
            this.PictureBoxTimeCorrection.Location = new System.Drawing.Point(138, 154);
            this.PictureBoxTimeCorrection.Name = "PictureBoxTimeCorrection";
            this.PictureBoxTimeCorrection.Size = new System.Drawing.Size(19, 19);
            this.PictureBoxTimeCorrection.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PictureBoxTimeCorrection.TabIndex = 8;
            this.PictureBoxTimeCorrection.TabStop = false;
            // 
            // LabelStatusTimeCorrection
            // 
            this.LabelStatusTimeCorrection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelStatusTimeCorrection.Location = new System.Drawing.Point(137, 153);
            this.LabelStatusTimeCorrection.Name = "LabelStatusTimeCorrection";
            this.LabelStatusTimeCorrection.Padding = new System.Windows.Forms.Padding(22, 0, 0, 0);
            this.LabelStatusTimeCorrection.Size = new System.Drawing.Size(233, 21);
            this.LabelStatusTimeCorrection.TabIndex = 4;
            this.LabelStatusTimeCorrection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ButtonVerify
            // 
            this.HelpProviderTimeCorrection.SetHelpString(this.ButtonVerify, "Performs verification of the URL to confirm it\'s validity.");
            this.ButtonVerify.Location = new System.Drawing.Point(211, 194);
            this.ButtonVerify.Name = "ButtonVerify";
            this.HelpProviderTimeCorrection.SetShowHelp(this.ButtonVerify, true);
            this.ButtonVerify.Size = new System.Drawing.Size(75, 23);
            this.ButtonVerify.TabIndex = 5;
            this.ButtonVerify.Text = "&Verify";
            this.ButtonVerify.UseVisualStyleBackColor = true;
            this.ButtonVerify.Click += new System.EventHandler(this.ButtonVerify_Click);
            // 
            // LabelTitleAbout
            // 
            this.LabelTitleAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.LabelTitleAbout.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTitleAbout.Location = new System.Drawing.Point(12, 8);
            this.LabelTitleAbout.Name = "LabelTitleAbout";
            this.LabelTitleAbout.Size = new System.Drawing.Size(329, 44);
            this.LabelTitleAbout.TabIndex = 7;
            this.LabelTitleAbout.Text = "Time Correction Validation";
            this.LabelTitleAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PictureBoxAbout
            // 
            this.PictureBoxAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.PictureBoxAbout.Image = ((System.Drawing.Image)(resources.GetObject("PictureBoxAbout.Image")));
            this.PictureBoxAbout.Location = new System.Drawing.Point(347, 3);
            this.PictureBoxAbout.Name = "PictureBoxAbout";
            this.PictureBoxAbout.Size = new System.Drawing.Size(54, 54);
            this.PictureBoxAbout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PictureBoxAbout.TabIndex = 12;
            this.PictureBoxAbout.TabStop = false;
            // 
            // LabelBannerAbout
            // 
            this.LabelBannerAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.LabelBannerAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelBannerAbout.Location = new System.Drawing.Point(0, 0);
            this.LabelBannerAbout.Name = "LabelBannerAbout";
            this.LabelBannerAbout.Size = new System.Drawing.Size(404, 60);
            this.LabelBannerAbout.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(23, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(363, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "In order to add time correction, you must first validate that the server can hand" +
    "le this function. Please input the complete server URL.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ErrorProviderTimeCorrection
            // 
            this.ErrorProviderTimeCorrection.ContainerControl = this;
            // 
            // ComboBoxUrlTimeCorrection
            // 
            this.ComboBoxUrlTimeCorrection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.ComboBoxUrlTimeCorrection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboBoxUrlTimeCorrection.FormattingEnabled = true;
            this.HelpProviderTimeCorrection.SetHelpString(this.ComboBoxUrlTimeCorrection, "Enter the server URL that you wish to enable Time Correction.");
            this.ComboBoxUrlTimeCorrection.Location = new System.Drawing.Point(137, 119);
            this.ComboBoxUrlTimeCorrection.Name = "ComboBoxUrlTimeCorrection";
            this.HelpProviderTimeCorrection.SetShowHelp(this.ComboBoxUrlTimeCorrection, true);
            this.ComboBoxUrlTimeCorrection.Size = new System.Drawing.Size(233, 21);
            this.ComboBoxUrlTimeCorrection.TabIndex = 2;
            // 
            // FormTimeCorrection
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(404, 229);
            this.Controls.Add(this.ComboBoxUrlTimeCorrection);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LabelTitleAbout);
            this.Controls.Add(this.PictureBoxAbout);
            this.Controls.Add(this.LabelBannerAbout);
            this.Controls.Add(this.PictureBoxTimeCorrection);
            this.Controls.Add(this.LabelStatusTimeCorrection);
            this.Controls.Add(this.LabelStatusDescriptionTimeCorrection);
            this.Controls.Add(this.LabelServerHttpUrlTimeCorrection);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonVerify);
            this.Controls.Add(this.ButtonOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTimeCorrection";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "_time correction_";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTimeCorrection_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTimeCorrection_FormClosed);
            this.Load += new System.EventHandler(this.FormTimeCorrection_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxTimeCorrection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProviderTimeCorrection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonCancel;
        private System.ComponentModel.BackgroundWorker WorkerWaitForCheck;
        private System.Windows.Forms.Label LabelServerHttpUrlTimeCorrection;
        private System.Windows.Forms.ImageList ImageListErrorProvider;
        private System.Windows.Forms.Label LabelStatusDescriptionTimeCorrection;
        private System.Windows.Forms.PictureBox PictureBoxTimeCorrection;
        private System.Windows.Forms.Label LabelStatusTimeCorrection;
        private System.Windows.Forms.Button ButtonVerify;
        private System.Windows.Forms.Label LabelTitleAbout;
        private System.Windows.Forms.PictureBox PictureBoxAbout;
        private System.Windows.Forms.Label LabelBannerAbout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider ErrorProviderTimeCorrection;
        internal System.Windows.Forms.ComboBox ComboBoxUrlTimeCorrection;
        private System.Windows.Forms.HelpProvider HelpProviderTimeCorrection;
    }
}