using System;
using System.Windows.Forms;

using KeePass.UI;
using KeePassLib;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Provides a column to Keepass showing the TOTP Status for an entry.
    /// </summary>
    internal class TrayTOTP_TOTPStatusColumn : ColumnProvider
    {
        private readonly KeeTrayTOTPExt _plugin;

        internal TrayTOTP_TOTPStatusColumn(KeeTrayTOTPExt plugin)
        {
            _plugin = plugin;
        }

        private static readonly string[] _columnName = new[] { Localization.Strings.ColumnTOTPStatus };

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
            if (pe == null)
            {
                return string.Empty;
            }

            var settingsCheck = _plugin.SettingsCheck(pe);
            var seedCheck = _plugin.SeedCheck(pe);

            if (settingsCheck  && seedCheck)
            {
                if (_plugin.SettingsValidate(pe))
                {
                    if (_plugin.SeedValidate(pe))
                    {
                        return Localization.Strings.TOTPEnabled;
                    }
                    return Localization.Strings.ErrorBadSeed;
                }
                return Localization.Strings.ErrorBadSettings;
            }
            return (settingsCheck || seedCheck) ? Localization.Strings.ErrorStorage : string.Empty;
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
            if (columnName == null)
            {
                throw new ArgumentNullException("columnName");
            }

            UIUtil.ShowDialogAndDestroy(new SetupTOTP(_plugin, pe));
        }
    }
}
