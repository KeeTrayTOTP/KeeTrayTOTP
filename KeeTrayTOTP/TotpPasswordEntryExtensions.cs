using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;
using KeeTrayTOTP.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeeTrayTOTP
{
    public static class TotpPasswordEntryExtensions
    {
        /// <summary>
        /// Static reference to the pluginhost (to ease calling these extensions).
        /// Must be set at startup and freed at Terminate.
        /// </summary>
        public static IPluginHost PluginHost { get; set; }

        private static readonly IReadOnlyCollection<string> setstat_allowed_lengths = new[] { "6", "7", "8", "S" };

        internal static bool CanGenerateTotp(this PwEntry pe)
        {
            return pe.HasTotpSettings() && pe.HasTotpSeed() && pe.HasValidTotpSettings();
        }

        /// <summary>
        /// Check if specified Entry contains Settings that are not null.
        /// </summary>
        /// <param name="pe">Pasword Entry.</param>
        /// <returns>Presence of Settings.</returns>
        internal static bool HasTotpSettings(this PwEntry pe)
        {
            return pe.Strings.Exists(PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings));
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal static bool HasValidTotpSettings(this PwEntry pe)
        {
            bool validInterval;
            bool validLength;
            bool validUrl;

            return HasValidTotpSettings(pe, out validInterval, out validLength, out validUrl);
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid. The URL status is available as an out boolean.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <param name="isUrlValid">Url Validity.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal static bool HasValidTotpSettings(this PwEntry pe, out bool isUrlValid)
        {
            bool validInterval; bool validLength; //Dummies

            return HasValidTotpSettings(pe, out validInterval, out validLength, out isUrlValid);
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid. All settings statuses are available as out booleans.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <param name="isIntervalValid">Interval Validity.</param>
        /// <param name="isLengthValid">Length Validity.</param>
        /// <param name="isUrlValid">Url Validity.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal static bool HasValidTotpSettings(this PwEntry pe, out bool isIntervalValid, out bool isLengthValid, out bool isUrlValid)
        {
            bool settingsValid;
            try
            {
                string[] settings = GetTotpSettings(pe);

                isIntervalValid = IntervalIsValid(settings);
                isLengthValid = LengthIsValid(settings);

                settingsValid = isIntervalValid && isLengthValid;

                isUrlValid = UrlIsValid(settings);
            }
            catch (Exception)
            {
                isIntervalValid = false;
                isLengthValid = false;
                isUrlValid = false;
                settingsValid = false;
            }
            return settingsValid;
        }

        private static bool UrlIsValid(string[] settings)
        {
            if (settings.Length < 3)
            {
                return false;
            }

            return settings[2].StartsWith("http://") || settings[2].StartsWith("https://");
        }

        private static bool LengthIsValid(string[] settings)
        {
            if (settings.Length < 2)
            {
                return false;
            }

            if (!setstat_allowed_lengths.Contains(settings[1]))
            {
                return false;
            }

            return true;
        }

        private static bool IntervalIsValid(string[] settings)
        {
            if (settings.Length == 0)
            {
                return false;
            }

            short interval;
            if (!short.TryParse(settings[0], out interval))
            {
                return false;
            }

            if (interval < 0 && interval < 180)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the entry's Settings using the string name specified in the settings (or default).
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>String Array (Interval, Length, Url).</returns>
        internal static string[] GetTotpSettings(this PwEntry pe)
        {
            return pe.Strings.Get(PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSettings_StringName, Localization.Strings.TOTPSettings)).ReadString().Split(';');
        }

        /// <summary>
        /// Check if the specified Entry contains a Seed.
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Presence of the Seed.</returns>
        internal static bool HasTotpSeed(this PwEntry pe)
        {
            return pe.Strings.Exists(PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed));
        }

        /// <summary>
        /// Validates the entry's Seed making sure it's a valid Base32 string.
        /// </summary>
        /// <param name="passwordEntry">Password Entry.</param>
        /// <returns>Validity of the Seed's characters for Base32 format.</returns>
        internal static bool SeedValidate(this PwEntry passwordEntry)
        {
            string invalidCharacters;
            return SeedGet(passwordEntry).ReadString().ExtWithoutSpaces().IsBase32(out invalidCharacters);
        }

        /// <summary>
        /// Validates the entry's Seed making sure it's a valid Base32 string. Invalid characters are available as out string.
        /// </summary>
        /// <param name="passwordEntry">Password Entry.</param>
        /// <param name="invalidChars">Password Entry.</param>
        /// <returns>Validity of the Seed's characters.</returns>
        internal static bool SeedValidate(this PwEntry passwordEntry, out string invalidChars)
        {
            return SeedGet(passwordEntry).ReadString().ExtWithoutSpaces().IsBase32(out invalidChars);
        }

        /// <summary>
        /// Get the entry's Seed using the string name specified in the settings (or default).
        /// </summary>
        /// <param name="pe">Password Entry.</param>
        /// <returns>Protected Seed.</returns>
        internal static ProtectedString SeedGet(this PwEntry pe)
        {
            return pe.Strings.Get(PluginHost.CustomConfig.GetString(KeeTrayTOTPExt.setname_string_TOTPSeed_StringName, Localization.Strings.TOTPSeed));
        }
    }
}
