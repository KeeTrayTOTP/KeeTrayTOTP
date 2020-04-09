using System;
using System.Windows.Forms;
using KeePass.UI;
using QRCoder;

namespace KeeTrayTOTP
{
    public partial class ShowQR : Form
    {
        public string Seed { get; set; }

        public ShowQR()
        {
            InitializeComponent();
        }

        private void ShowQR_Load(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            GenerateQRCode();
        }

        private void GenerateQRCode()
        {
            var code = string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}", Uri.EscapeDataString(IssuerText.Text),
                Uri.EscapeDataString(UsernameText.Text), Seed);

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(6);

                QROutputPicture.Image = qrCodeImage;
            }
        }

        private void RegenerateButton_Click(object sender, EventArgs e)
        {
            GenerateQRCode();
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
    }
}
