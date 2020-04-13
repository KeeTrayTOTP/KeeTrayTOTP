using System;
using System.Windows.Forms;

using KeePass.UI;
using KeePass.Plugins;
using KeePassLib;

using KeeTrayTOTP.Libraries;
using System.Diagnostics;

namespace KeeTrayTOTP
{
    internal class TrayTOTP_CustomColumn : ColumnProvider
    {
        /// <summary>
        /// Reference to the plugin host for access to KeePass functions.
        /// </summary>
        private readonly IPluginHost _pluginHost;

        /// <summary>
        /// Reference to the main plugin class.
        /// </summary>
        private readonly KeeTrayTOTPExt _plugin;

        /// <summary>
        /// Provides support to add a custom column to KeePass, in this case the TOTP Provider Column.
        /// </summary>
        /// <param name="plugin">Handle to the plugin class.</param>
        internal TrayTOTP_CustomColumn(KeeTrayTOTPExt plugin)
        {
            _plugin = plugin;
            _pluginHost = _plugin.PluginHost;
        }

        /// <summary>
        /// Informs Keepass of the Column Names, in the case this provider handles more than one column.
        /// </summary>
        public override string[] ColumnNames
        {
            get { return new[] { Localization.Strings.TOTP }; }
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
            if (strColumnName == null) { Debug.Assert(false); return string.Empty; }
            if (strColumnName != Localization.Strings.TOTP) return string.Empty;
            if (pe == null) { Debug.Assert(false); return string.Empty; }

            if (pe.HasTotpSettings() && pe.HasTotpSeed())
            {
                if (pe.HasValidTotpSettings())
                {
                    string[] settings = pe.GetTotpSettings();

                    TOTPProvider totpGenerator = new TOTPProvider(settings, _plugin.TimeCorrections);

                    if (pe.SeedValidate())
                    {
                        return totpGenerator.GenerateByByte(Base32.Decode(pe.SeedGet().ReadString().ExtWithoutSpaces())) + (_pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnTimer_Visible, true) ? totpGenerator.Timer.ToString().ExtWithParenthesis().ExtWithSpaceBefore() : string.Empty);
                    }
                    return Localization.Strings.ErrorBadSeed;
                }
                return Localization.Strings.ErrorBadSettings;
            }
            return pe.HasTotpSettings() || pe.HasTotpSeed() ? Localization.Strings.ErrorStorage : string.Empty;
        }

        /// <summary>
        /// Informs KeePass if PerformCellAction must be called when the cell is double clicked.
        /// </summary>
        /// <param name="strColumnName">Column Name.</param>
        public override bool SupportsCellAction(string strColumnName)
        {
            return true;
        }

        /// <summary>
        /// Happens when a cell of the column is double-clicked.
        /// </summary>
        /// <param name="columnName">Column's name.</param>
        /// <param name="pe">Entry associated with the clicked cell.</param>
        public override void PerformCellAction(string columnName, PwEntry pe)
        {
            if (_pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnCopy_Enable, true))
            {
                _plugin.TOTPCopyToClipboard(pe);
            }
        }
    }
}
