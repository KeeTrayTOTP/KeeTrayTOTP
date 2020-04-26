using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using KeePass.UI;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Form providing information on the plugin.
    /// </summary>
    internal partial class FormAbout : Form
    {
        /// <summary>
        /// Tray TOTP Support Url
        /// </summary>
        private static readonly Uri SupportUrl = new Uri("https://github.com/KeeTrayTOTP/KeeTrayTOTP/issues", UriKind.Absolute);

        /// <summary>
        /// Windows Form Constructor.
        /// </summary>
        internal FormAbout()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Windows Form Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAbout_Load(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            Text = Localization.Strings.About + " - " + Localization.Strings.TrayTOTPPlugin;
            ListViewAbout.Items[0].SubItems.Add(AssemblyTitle);
            ListViewAbout.Items[1].SubItems.Add(AssemblyCompany);
            ListViewAbout.Items[2].SubItems.Add(AssemblyVersion);
            ListViewAbout.Items[3].SubItems.Add(AssemblyTrademark);
            ListViewAbout.Items[4].SubItems.Add(SupportUrl.AbsoluteUri);
            LabelCopyright.Text = AssemblyCopyright;

            SetHyperlinkStyle(4, Color.Blue);
        }

        /// <summary>
        /// Gets the assembly's title
        /// </summary>
        internal string AssemblyTitle
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>();
                return attribute != null ? attribute.Title : null;
            }
        }

        /// <summary>
        /// Gets the assembly's version
        /// </summary>
        internal string AssemblyVersion
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                return attribute != null ? attribute.InformationalVersion: null;
            }
        }

        /// <summary>
        /// Gets the assembly's description
        /// </summary>
        internal string AssemblyDescription
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>();
                return attribute != null ? attribute.Description : null;
            }
        }

        /// <summary>
        /// Gets the assembly's product name
        /// </summary>
        internal string AssemblyProduct
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>();
                return attribute != null ? attribute.Product : null;
            }
        }

        /// <summary>
        /// Gets the assembly's copyright
        /// </summary>
        internal string AssemblyCopyright
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>();
                return attribute != null ? attribute.Copyright : null;
            }
        }

        /// <summary>
        /// Gets the assembly's trademark
        ///  </summary>
        internal string AssemblyTrademark
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTrademarkAttribute>();
                return attribute != null ? attribute.Trademark : null;
            }
        }

        /// <summary>
        /// Gets the assembly's company name
        /// </summary>
        internal string AssemblyCompany
        {
            get
            {
                var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>();
                return attribute != null ? attribute.Company : null;
            }
        }

        private void FormAbout_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        /// <summary>
        /// Windows Form Mouse Click Handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewAbout_MouseClick(object sender, MouseEventArgs e)
        {
            var hit = ListViewAbout.HitTest(e.Location);
            if (hit.Item != null && hit.Item == ListViewAbout.Items[4] && hit.SubItem == ListViewAbout.Items[4].SubItems[1])
            { 
                System.Diagnostics.Process.Start(SupportUrl.ToString());
            }
        }
        /// <summary>
        /// Windows Form Row Subitem To Underlined Blue Color Font.
        /// </summary>
        /// <param name="RowIndex"></param>
        /// /// <param name="color"></param>
        private void SetHyperlinkStyle(int RowIndex, Color color)
        {
            ListViewAbout.Items[RowIndex].UseItemStyleForSubItems = false;
            ListViewAbout.Items[RowIndex].SubItems[1].Font = new Font("Microsoft Sans Serif", 8, FontStyle.Underline);
            ListViewAbout.Items[RowIndex].SubItems[1].ForeColor = color;

        }

        /// <summary>
        /// Windows Form Mouse Move Handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewAbout_MouseMove(object sender, MouseEventArgs e)
        {
            var hit = ListViewAbout.HitTest(e.Location);
            if (hit.SubItem != null && hit.SubItem == hit.Item.SubItems[1] && hit.Item == ListViewAbout.Items[4]) {
                ListViewAbout.Cursor = Cursors.Hand;
            } else {
                ListViewAbout.Cursor = Cursors.Default; 
            }
        }
    }
}