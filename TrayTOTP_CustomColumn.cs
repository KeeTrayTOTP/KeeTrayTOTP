using System;
using System.Windows.Forms;

using KeePass.UI;
using KeePass.Plugins;
using KeePassLib;

using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Main Plugin Class
    /// </summary>
    internal sealed partial class KeeTrayTOTPExt
    {
        internal class TrayTOTP_CustomColumn : ColumnProvider
        {
            /// <summary>
            /// Reference to the plugin host for access to KeePass functions.
            /// </summary>
            private readonly IPluginHost m_host;

            /// <summary>
            /// Reference to the main plugin class.
            /// </summary>
            private readonly KeeTrayTOTPExt plugin;

            /// <summary>
            /// Provides support to add a custom column to KeePass, in this case the TOTP Provider Column.
            /// </summary>
            /// <param name="Plugin">Handle to the plugin class.</param>
            internal TrayTOTP_CustomColumn(KeeTrayTOTPExt Plugin)
            {
                plugin = Plugin;
                m_host = plugin.m_host;
            }

            /// <summary>
            /// Column Names, in the case this provider handles more than one column.
            /// </summary>
            private readonly string[] ColumnName = new[] {TrayTOTP_CustomColumn_Localization.strTOTP};

            /// <summary>
            /// Informs Keepass of the Column Names, in the case this provider handles more than one column.
            /// </summary>
            public override string[] ColumnNames
            {
                get { return ColumnName; }
            }

            /// <summary>
            /// Informs KeePass what alignment the column's data should have.
            /// </summary>
            public override HorizontalAlignment TextAlign
            {
                get { return HorizontalAlignment.Left; }
            }

            /// <summary>
            /// Tells KeePass what to display in the column.
            /// </summary>
            /// <param name="strColumnName"></param>
            /// <param name="pe"></param>
            /// <returns>String displayed in the columns.</returns>
            public override string GetCellData(string strColumnName, PwEntry pe)
            {
                if (strColumnName == null) throw new ArgumentNullException("strColumnName");
                if (pe == null) throw new ArgumentNullException("pe");
                if (plugin.SettingsCheck(pe) && plugin.SeedCheck(pe))
                {
                    bool ValidInterval;
                    bool ValidLength;
                    bool ValidUrl;
                    if (plugin.SettingsValidate(pe, out ValidInterval, out ValidLength, out ValidUrl))
                    {
                        string[] Settings = plugin.SettingsGet(pe);
                        var TOTPGenerator = new TOTPProvider(Convert.ToInt16(Settings[0]), Convert.ToInt16(Settings[1]));
                        if (ValidUrl)
                        {
                            var TimeCorrection = plugin.TimeCorrections[Settings[2]];
                            if (TimeCorrection == null) return TrayTOTP_CustomColumn_Localization.strWarningBadUrl;
                            TOTPGenerator.TimeCorrection = TimeCorrection.TimeCorrection;
                        }
                        if (plugin.SeedValidate(pe))
                        {
                            return TOTPGenerator.Generate(Base32.Decode(plugin.SeedGet(pe).ReadString().ExtWithoutSpaces())) + (m_host.CustomConfig.GetBool(setname_bool_TOTPColumnTimer_Visible, true) ? TOTPGenerator.Timer.ToString().ExtWithParenthesis().ExtWithSpaceBefore() : string.Empty);
                        }
                        return TrayTOTP_CustomColumn_Localization.strWarningBadSeed;
                    }
                    return TrayTOTP_CustomColumn_Localization.strWarningBadSet;
                }
                return (plugin.SettingsCheck(pe) || plugin.SeedCheck(pe) ? TrayTOTP_CustomColumn_Localization.strWarningStorage : string.Empty);
            }

            /// <summary>
            /// Informs KeePass if PerformCellAction must be called when the cell is double clicked.
            /// </summary>
            /// <param name="strColumnName">Column Name.</param>
            /// <returns></returns>
            public override bool SupportsCellAction(string strColumnName)
            {
                if (strColumnName == null) throw new ArgumentNullException("strColumnName");
                return true;
            }

            /// <summary>
            /// Happens when a cell of the column is double-clicked.
            /// </summary>
            /// <param name="strColumnName">Column's name.</param>
            /// <param name="pe">Entry associated with the clicked cell.</param>
            public override void PerformCellAction(string strColumnName, PwEntry pe)
            {
                if (strColumnName == null) throw new ArgumentNullException("strColumnName");
                if (m_host.CustomConfig.GetBool(setname_bool_TOTPColumnCopy_Enable, true)) plugin.TOTPCopyToClipboard(pe);
            }
        }
    }
}
