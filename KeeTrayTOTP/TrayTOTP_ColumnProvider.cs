using System;
using System.Windows.Forms;

using KeePass.UI;
using KeePassLib;
using KeeTrayTOTP.Libraries;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Provides columns to Keepass showing the TOTP Code / TOTP Status for an entry.
    /// </summary>
    internal class TrayTOTP_ColumnProvider : ColumnProvider
    {
        private readonly KeeTrayTOTPExt _plugin;

        internal TrayTOTP_ColumnProvider(KeeTrayTOTPExt plugin)
        {
            _plugin = plugin;
        }

        private static readonly string[] _columnName = new[] { Localization.Strings.ColumnTOTPCode, Localization.Strings.ColumnTOTPStatus };

        public override string[] ColumnNames
        {
            get { return _columnName; }
        }

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
            if (columnName == Localization.Strings.ColumnTOTPCode)
            {
                return GetCellDataInternal(pe, GetInnerValueCode);
            }
            else if (columnName == Localization.Strings.ColumnTOTPStatus)
            {
                return GetCellDataInternal(pe, GetInnerValueStatus);
            }

            return string.Empty;
        }

        private string GetCellDataInternal(PwEntry pe, Func<PwEntry, string> innerValueFunc)
        {
            var settingsCheck = _plugin.SettingsCheck(pe);
            var seedCheck = _plugin.SeedCheck(pe);

            if (settingsCheck && seedCheck)
            {
                if (_plugin.SettingsValidate(pe))
                {
                    if (_plugin.SeedValidate(pe))
                    {
                        return innerValueFunc(pe);
                    }
                    return Localization.Strings.ErrorBadSeed;
                }
                return Localization.Strings.ErrorBadSettings;
            }
            return (settingsCheck || seedCheck) ? Localization.Strings.ErrorStorage : string.Empty;
        }

        private static string GetInnerValueStatus(PwEntry entry)
        {
            return Localization.Strings.TOTPEnabled;
        }

        private string GetInnerValueCode(PwEntry entry)
        {
            string[] settings = _plugin.SettingsGet(entry);
            var totpGenerator = new TOTPProvider(settings, ref _plugin.TimeCorrections);
            return totpGenerator.GenerateByByte(Base32.Decode(_plugin.SeedGet(entry).ReadString().ExtWithoutSpaces())) + (_plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnTimer_Visible, true) ? totpGenerator.Timer.ToString().ExtWithParenthesis().ExtWithSpaceBefore() : string.Empty);
        }

        /// <summary>
        /// Informs KeePass if PerformCellAction must be called when the cell is double clicked.
        /// </summary>
        /// <param name="columnName">Column Name.</param>
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
            if (_plugin.PluginHost.CustomConfig.GetBool(KeeTrayTOTPExt.setname_bool_TOTPColumnCopy_Enable, true))
            {
                _plugin.TOTPCopyToClipboard(pe);
            }
        }
    }
}
