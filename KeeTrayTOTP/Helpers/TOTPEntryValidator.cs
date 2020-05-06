using KeePassLib;
using KeeTrayTOTP.Libraries;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace KeeTrayTOTP.Helpers
{
    // TODO: stil not the right name: Also gets settings?
    public class TOTPEntryValidator
    {
        private static readonly ReadOnlyCollection<string> AllowedLengths = new ReadOnlyCollection<string>(new[] { "6", "7", "8", "S" });
        private readonly ISettings _settings;

        public TOTPEntryValidator(ISettings settings)
        {
            this._settings = settings;
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid.
        /// </summary>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry entry)
        {
            bool validInterval;
            bool validLength;
            bool validUrl;

            return SettingsValidate(entry, out validInterval, out validLength, out validUrl);
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid. The URL status is available as an out boolean.
        /// </summary>
        /// <param name="isUrlValid">Url Validity.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry entry, out bool isUrlValid)
        {
            bool validInterval; bool validLength; //Dummies

            return SettingsValidate(entry, out validInterval, out validLength, out isUrlValid);
        }

        /// <summary>
        /// Check if specified Entry's Interval and Length are valid. All settings statuses are available as out booleans.
        /// </summary>
        /// <param name="isIntervalValid">Interval Validity.</param>
        /// <param name="isLengthValid">Length Validity.</param>
        /// <param name="isUrlValid">Url Validity.</param>
        /// <returns>Error(s) while validating Interval or Length.</returns>
        internal bool SettingsValidate(PwEntry entry, out bool isIntervalValid, out bool isLengthValid, out bool isUrlValid)
        {
            bool settingsValid;
            try
            {
                string[] settings = SettingsGet(entry);

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

        /// <summary>
        /// Get the entry's Settings, or return defaults
        /// </summary>
        /// <returns>String Array (Interval, Length, Url).</returns>
        internal string[] SettingsGet(PwEntry entry)
        {
            return HasExplicitSettings(entry)
                ? entry.Strings.Get(_settings.TOTPSettingsStringName).ReadString().Split(';')
                : new[] { "30", "6" };
        }

        /// <summary>
        /// Check if the specified Entry contains a Seed.
        /// </summary>
        /// <returns>Presence of the Seed.</returns>
        internal bool HasSeed(PwEntry entry)
        {
            return entry.Strings.Exists(_settings.TOTPSeedStringName);
        }

        /// <summary>
        /// Validates the entry's Seed making sure it's a valid Base32 string.
        /// </summary>
        /// <returns>Validity of the Seed's characters for Base32 format.</returns>
        internal bool SeedValidate(PwEntry entry)
        {
            return GetCleanSeed(entry).IsBase32();
        }

        /// <summary>
        /// Validates the entry's Seed making sure it's a valid Base32 string. Invalid characters are available as out string.
        /// </summary>
        /// <param name="invalidCharacters">Password Entry.</param>
        /// <returns>Validity of the Seed's characters.</returns>
        internal bool SeedValidate(PwEntry entry, out string invalidCharacters)
        {
            return GetCleanSeed(entry).IsBase32(out invalidCharacters);
        }

        /// <summary>
        /// Get the entry's Seed using the string name specified in the settings (or default).
        /// </summary>
        /// <returns>Protected Seed.</returns>
        internal string GetCleanSeed(PwEntry entry)
        {
            var rawSeed = entry.Strings.Get(_settings.TOTPSeedStringName).ReadString();
            return Regex.Replace(rawSeed, @"\s+", "").TrimEnd('=');
        }

        /// <summary>
        /// Get the entry's Seed using the string name specified in the settings (or default).
        /// </summary>
        /// <returns>Protected Seed.</returns>
        internal byte[] GetByteSeed(PwEntry entry)
        {
            return Base32.Decode(GetCleanSeed(entry));
        }

        internal bool CanGenerateTOTP(PwEntry entry)
        {
            return HasSeed(entry) && SettingsValidate(entry) && SeedValidate(entry);
        }

        /// <summary>
        /// Check if specified Entry contains Settings that are not null.
        /// </summary>
        /// <returns>Presence of Settings.</returns>
        internal bool HasExplicitSettings(PwEntry entry)
        {
            return entry.Strings.Exists(_settings.TOTPSettingsStringName);
        }

        internal KeyUri ReadAsKeyUri(PwEntry entry)
        {
            if (entry.Strings.Exists(_settings.TOTPKeyUriStringName))
            {
                // load in this format
                return new KeyUri(new Uri(entry.Strings.Get(_settings.TOTPKeyUriStringName).ReadString()));
            }
            else if (SettingsValidate(entry) && HasSeed(entry))
            {
                if (!SeedValidate(entry))
                {
                    throw new InvalidOperationException("Cannot generate KeyUri for entry");
                }

                var settings = SettingsGet(entry);
                var seed = GetCleanSeed(entry);

                return KeyUri.CreateFromLegacySettings(settings, seed);
            }

            return null;
        }

        internal void SetKeyUri(PwEntry entry, KeyUri keyUri)
        {
            if (_settings.PreferKeyUri)
            {
                entry.Strings.Set(_settings.TOTPKeyUriStringName, new KeePassLib.Security.ProtectedString(true, keyUri.GetUri().AbsoluteUri));
                entry.Strings.Remove(_settings.TOTPSeedStringName);
                entry.Strings.Remove(_settings.TOTPSettingsStringName);
            }
            else
            {
                entry.Strings.Set(_settings.TOTPSeedStringName, new KeePassLib.Security.ProtectedString(true, keyUri.Secret));
                string settings = keyUri.ToLegacySettings();
                entry.Strings.Set(_settings.TOTPSettingsStringName, new KeePassLib.Security.ProtectedString(true, settings));
                entry.Strings.Remove(_settings.TOTPKeyUriStringName);
            }
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

            if (!AllowedLengths.Contains(settings[1]))
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

            if (interval < 0)
            {
                return false;
            }

            return true;
        }
    }
}
