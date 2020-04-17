using System;
using System.Windows.Forms;

using KeePass.UI;
using KeePass.Plugins;
using KeePassLib;

using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Provides a column to Keepass showing the TOTP Code for an entry.
    /// </summary>
    internal class TrayTOTP_TOTPCodeColumn : ColumnProvider
    {
        /// <summary>
        /// Reference to the plugin host for access to KeePass functions.
        /// </summary>
        private readonly IPluginHost _pluginHost;

        /// <summary>
        /// Reference to the main plugin class.
        /// </summary>
        private readonly KeeTrayTOTPExt _plugin;

        internal TrayTOTP_TOTPCodeColumn(KeeTrayTOTPExt plugin)
        {
            _plugin = plugin;
            _pluginHost = _plugin.PluginHost;
        }

        /// <summary>
        /// Column Names, in the case this provider handles more than one column.
        /// </summary>
        private static readonly string[] _columnName = new[] { Localization.Strings.ColumnTOTPCode };

        /// <summary>
        /// Informs Keepass of the Column Names, in the case this provider handles more than one column.
        /// </summary>
        public override string[] ColumnNames
        {
            get { return _columnName; }
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
        /// <param name="columnName"></param>
        /// <param name="pe"></param>
        /// <returns>String displayed in the columns.</returns>
        public override string GetCellData(string columnName, PwEntry pe)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException("columnName");
            }

            if (pe == null)
            {
                throw new ArgumentNullException("pe");
            }

            if (_plugin.SettingsCheck(pe) && _plugin.SeedCheck(pe))
            {
                bool validInterval;
                bool validLength;
                bool validUrl;
                if (_plugin.SettingsValidate(pe, out validInterval, out validLength, out validUrl))
                {
                    string[] settings = _plugin.SettingsGet(pe);

                    TOTPProvider totpGenerator = new TOTPProvider(settings, ref _plugin.TimeCorrections);

                    if (_plugin.SeedValidate(pe))
                    {
                        return totpGenerator.GenerateByByte(Base32.Decode(_plugin.SeedGet(pe).ReadString().ExtWithoutSpaces())) + (_pluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnTimer_Visible, true) ? totpGenerator.Timer.ToString().ExtWithParenthesis().ExtWithSpaceBefore() : string.Empty);
                    }
                    return Localization.Strings.ErrorBadSeed;
                }
                return Localization.Strings.ErrorBadSettings;
            }
            return _plugin.SettingsCheck(pe) || _plugin.SeedCheck(pe) ? Localization.Strings.ErrorStorage : string.Empty;
        }

        /// <summary>
        /// Informs KeePass if PerformCellAction must be called when the cell is double clicked.
        /// </summary>
        /// <param name="columnName">Column Name.</param>
        /// <returns></returns>
        public override bool SupportsCellAction(string columnName)
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
