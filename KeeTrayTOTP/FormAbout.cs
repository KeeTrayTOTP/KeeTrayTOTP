using System;
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
        /// Plugin Host.
        /// </summary>
        private readonly KeeTrayTOTPExt _plugin;

        /// <summary>
        /// Windows Form Constructor.
        /// </summary>
        /// <param name="plugin">Plugin Host.</param>
        internal FormAbout(KeeTrayTOTPExt plugin)
        {
            _plugin = plugin;
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
            ListViewAbout.Items[4].SubItems.Add(KeeTrayTOTPExt.SupportUrl);
            LabelCopyright.Text = AssemblyCopyright;
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
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Gets the assembly's trademark
        ///  </summary>
        internal string AssemblyTrademark
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyTrademarkAttribute)attributes[0]).Trademark;
            }
        }

        /// <summary>
        /// Gets the assembly's company name
        /// </summary>
        internal string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        private void FormAbout_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }
    }
}