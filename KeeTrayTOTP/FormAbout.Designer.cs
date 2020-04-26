namespace KeeTrayTOTP
{
    partial class FormAbout
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Title");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Company");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Version");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Build Date");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Support");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            this.LabelTitleAbout = new System.Windows.Forms.Label();
            this.LabelBannerAbout = new System.Windows.Forms.Label();
            this.ListViewAbout = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ButtonClose = new System.Windows.Forms.Button();
            this.LabelCopyright = new System.Windows.Forms.Label();
            this.PictureBoxAbout = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbout)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelTitleAbout
            // 
            this.LabelTitleAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.LabelTitleAbout.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTitleAbout.Location = new System.Drawing.Point(13, 9);
            this.LabelTitleAbout.Name = "LabelTitleAbout";
            this.LabelTitleAbout.Size = new System.Drawing.Size(363, 44);
            this.LabelTitleAbout.TabIndex = 4;
            this.LabelTitleAbout.Text = "Tray TOTP Plugin";
            this.LabelTitleAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelBannerAbout
            // 
            this.LabelBannerAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.LabelBannerAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelBannerAbout.Location = new System.Drawing.Point(0, 0);
            this.LabelBannerAbout.Name = "LabelBannerAbout";
            this.LabelBannerAbout.Size = new System.Drawing.Size(439, 60);
            this.LabelBannerAbout.TabIndex = 3;
            // 
            // ListViewAbout
            // 
            this.ListViewAbout.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.ListViewAbout.FullRowSelect = true;
            this.ListViewAbout.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.ListViewAbout.HideSelection = false;
            this.ListViewAbout.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.ListViewAbout.Location = new System.Drawing.Point(14, 73);
            this.ListViewAbout.Name = "ListViewAbout";
            this.ListViewAbout.ShowGroups = false;
            this.ListViewAbout.ShowItemToolTips = true;
            this.ListViewAbout.Size = new System.Drawing.Size(410, 133);
            this.ListViewAbout.TabIndex = 0;
            this.ListViewAbout.UseCompatibleStateImageBehavior = false;
            this.ListViewAbout.View = System.Windows.Forms.View.Details;
            this.ListViewAbout.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListViewAbout_MouseClick);
            this.ListViewAbout.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListViewAbout_MouseMove);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 300;
            // 
            // ButtonClose
            // 
            this.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonClose.Location = new System.Drawing.Point(349, 219);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(75, 23);
            this.ButtonClose.TabIndex = 1;
            this.ButtonClose.Text = "&Close";
            this.ButtonClose.UseVisualStyleBackColor = true;
            // 
            // LabelCopyright
            // 
            this.LabelCopyright.Location = new System.Drawing.Point(14, 219);
            this.LabelCopyright.Name = "LabelCopyright";
            this.LabelCopyright.Size = new System.Drawing.Size(329, 23);
            this.LabelCopyright.TabIndex = 2;
            this.LabelCopyright.Text = "_copyright_";
            this.LabelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PictureBoxAbout
            // 
            this.PictureBoxAbout.BackColor = System.Drawing.Color.DarkOrange;
            this.PictureBoxAbout.Image = ((System.Drawing.Image)(resources.GetObject("PictureBoxAbout.Image")));
            this.PictureBoxAbout.Location = new System.Drawing.Point(382, 3);
            this.PictureBoxAbout.Name = "PictureBoxAbout";
            this.PictureBoxAbout.Size = new System.Drawing.Size(54, 54);
            this.PictureBoxAbout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PictureBoxAbout.TabIndex = 5;
            this.PictureBoxAbout.TabStop = false;
            // 
            // FormAbout
            // 
            this.AcceptButton = this.ButtonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonClose;
            this.ClientSize = new System.Drawing.Size(439, 254);
            this.Controls.Add(this.PictureBoxAbout);
            this.Controls.Add(this.LabelCopyright);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.ListViewAbout);
            this.Controls.Add(this.LabelTitleAbout);
            this.Controls.Add(this.LabelBannerAbout);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "_about form_";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAbout_FormClosed);
            this.Load += new System.EventHandler(this.FormAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbout)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabelTitleAbout;
        private System.Windows.Forms.Label LabelBannerAbout;
        private System.Windows.Forms.ListView ListViewAbout;
        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.Label LabelCopyright;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.PictureBox PictureBoxAbout;
    }
}