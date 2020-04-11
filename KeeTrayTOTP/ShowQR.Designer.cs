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
            this.QROutputPicture.MinimumSize = new System.Drawing.Size(60, 60);
            this.QROutputPicture.Name = "QROutputPicture";
            this.QROutputPicture.Size = new System.Drawing.Size(360, 360);
            this.QROutputPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.QROutputPicture.TabIndex = 13;
            this.QROutputPicture.TabStop = false;
            // 
            // IssuerText
            // 
            this.IssuerText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IssuerText.Location = new System.Drawing.Point(107, 69);
            this.IssuerText.Name = "IssuerText";
            this.IssuerText.Size = new System.Drawing.Size(265, 20);
            this.IssuerText.TabIndex = 14;
            this.IssuerText.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // UsernameText
            // 
            this.UsernameText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UsernameText.Location = new System.Drawing.Point(107, 93);
            this.UsernameText.Name = "UsernameText";
            this.UsernameText.Size = new System.Drawing.Size(265, 20);
            this.UsernameText.TabIndex = 15;
            this.UsernameText.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // IssuerLabel
            // 
            this.IssuerLabel.AutoSize = true;
            this.IssuerLabel.Location = new System.Drawing.Point(15, 72);
            this.IssuerLabel.Name = "IssuerLabel";
            this.IssuerLabel.Size = new System.Drawing.Size(64, 13);
            this.IssuerLabel.TabIndex = 16;
            this.IssuerLabel.Text = "Issuer (Title)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "User (Subtitle)";
            // 
            // ShowQR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 490);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.IssuerLabel);
            this.Controls.Add(this.UsernameText);
            this.Controls.Add(this.IssuerText);
            this.Controls.Add(this.QROutputPicture);
            this.Controls.Add(this.TitleAboutLabel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 330);
            this.Name = "ShowQR";
            this.ShowInTaskbar = false;
            this.Text = "QR Code";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShowQR_FormClosed);
            this.Load += new System.EventHandler(this.ShowQR_Load);
            this.ResizeEnd += new System.EventHandler(this.OnResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.QROutputPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleAboutLabel;
        private System.Windows.Forms.PictureBox QROutputPicture;
        private System.Windows.Forms.Label IssuerLabel;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox IssuerText;
        public System.Windows.Forms.TextBox UsernameText;
    }
}