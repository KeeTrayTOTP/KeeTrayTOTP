using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
            GenerateQRCode();
        }

        private void GenerateQRCode()
        {
            var code = string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}", Uri.EscapeDataString(IssuerText.Text),
                Uri.EscapeDataString(UsernameText.Text), Seed);
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(6);
            QROutputPicture.Image = qrCodeImage;
        }

        private void RegenerateButton_Click(object sender, EventArgs e)
        {
            GenerateQRCode();
        }
    }
}
