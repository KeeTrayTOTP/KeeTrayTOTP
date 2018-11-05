namespace KeeTrayTOTP
{
    partial class ShowQR
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
            this.TitleAboutLabel = new System.Windows.Forms.Label();
            this.QROutputPicture = new System.Windows.Forms.PictureBox();
            this.IssuerText = new System.Windows.Forms.TextBox();
            this.UsernameText = new System.Windows.Forms.TextBox();
            this.IssuerLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RegenerateButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.QROutputPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // TitleAboutLabel
            // 
            this.TitleAboutLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TitleAboutLabel.BackColor = System.Drawing.Color.DarkOrange;
            this.TitleAboutLabel.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleAboutLabel.Location = new System.Drawing.Point(12, 9);
            this.TitleAboutLabel.Name = "TitleAboutLabel";
            this.TitleAboutLabel.Size = new System.Drawing.Size(360, 54);
            this.TitleAboutLabel.TabIndex = 12;
            this.TitleAboutLabel.TabStop = true;
            this.TitleAboutLabel.Text = "Show QR";
            this.TitleAboutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // QROutputPicture
            // 
            this.QROutputPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QROutputPicture.Location = new System.Drawing.Point(12, 118);
            this.QROutputPicture.Name = "QROutputPicture";
            this.QROutputPicture.Size = new System.Drawing.Size(360, 360);
            this.QROutputPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.QROutputPicture.TabIndex = 13;
            this.QROutputPicture.TabStop = false;
            // 
            // IssuerText
            // 
            this.IssuerText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IssuerText.Location = new System.Drawing.Point(107, 65);
            this.IssuerText.Name = "IssuerText";
            this.IssuerText.Size = new System.Drawing.Size(100, 20);
            this.IssuerText.TabIndex = 14;
            // 
            // UsernameText
            // 
            this.UsernameText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UsernameText.Location = new System.Drawing.Point(107, 92);
            this.UsernameText.Name = "UsernameText";
            this.UsernameText.Size = new System.Drawing.Size(100, 20);
            this.UsernameText.TabIndex = 15;
            // 
            // IssuerLabel
            // 
            this.IssuerLabel.AutoSize = true;
            this.IssuerLabel.Location = new System.Drawing.Point(13, 72);
            this.IssuerLabel.Name = "IssuerLabel";
            this.IssuerLabel.Size = new System.Drawing.Size(64, 13);
            this.IssuerLabel.TabIndex = 16;
            this.IssuerLabel.Text = "Issuer (Title)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "User (Subtitle)";
            // 
            // RegenerateButton
            // 
            this.RegenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RegenerateButton.Location = new System.Drawing.Point(224, 72);
            this.RegenerateButton.Name = "RegenerateButton";
            this.RegenerateButton.Size = new System.Drawing.Size(148, 40);
            this.RegenerateButton.TabIndex = 18;
            this.RegenerateButton.Text = "&Regenerate";
            this.RegenerateButton.UseVisualStyleBackColor = true;
            this.RegenerateButton.Click += new System.EventHandler(this.RegenerateButton_Click);
            // 
            // ShowQR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 490);
            this.Controls.Add(this.RegenerateButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.IssuerLabel);
            this.Controls.Add(this.UsernameText);
            this.Controls.Add(this.IssuerText);
            this.Controls.Add(this.QROutputPicture);
            this.Controls.Add(this.TitleAboutLabel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShowQR";
            this.ShowInTaskbar = false;
            this.Text = "ShowQR";
            this.Load += new System.EventHandler(this.ShowQR_Load);
            ((System.ComponentModel.ISupportInitialize)(this.QROutputPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleAboutLabel;
        private System.Windows.Forms.PictureBox QROutputPicture;
        private System.Windows.Forms.Label IssuerLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button RegenerateButton;
        public System.Windows.Forms.TextBox IssuerText;
        public System.Windows.Forms.TextBox UsernameText;
    }
}