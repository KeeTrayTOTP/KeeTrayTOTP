using System;
using System.Windows.Forms;
using KeePass.UI;
using QRCoder;

namespace KeeTrayTOTP
{
    public partial class ShowQR : Form
    {
        private readonly string seed;

        public ShowQR(string seed, string issuer, string username)
        {
            InitializeComponent();

            this.seed = seed;
            this.IssuerText.Text = issuer;
            this.UsernameText.Text = username;
        }

        private void ShowQR_Load(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            GenerateQRCode();
        }

        private void GenerateQRCode()
        {
            var code = string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}", Uri.EscapeDataString(IssuerText.Text), Uri.EscapeDataString(UsernameText.Text), this.seed);

            using (var qrGenerator = new QRCodeGenerator())
            {
                using (var qrCodeData = qrGenerator.CreateQrCode(code))
                {
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        // default width = 360, resulting in 6 pixels per module
                        int pixelsPerModule = Math.Max(3, QROutputPicture.Width / 60);
                        var oldImage = QROutputPicture.Image;
                        
                        QROutputPicture.Image = qrCode.GetGraphic(pixelsPerModule);
                        
                        if (oldImage != null)
                        {
                            oldImage.Dispose();
                        }
                    }
                }
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void ShowQR_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            GenerateQRCode();
        }

        private void OnResizeEnd(object sender, EventArgs e)
        {
            GenerateQRCode();
        }
    }
}
