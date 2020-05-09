using KeeTrayTOTP.Libraries;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace KeeTrayTOTP
{
    public class KeyUri
    {
        public enum TotpFormat
        {
            /// <summary>
            /// Default format
            /// </summary>
            RFC6238,
            /// <summary>
            /// Custom format used by steam
            /// </summary>
            Steam,
        }

        private static string[] ValidAlgorithms = new[] { "SHA1", "SHA256", "SHA512" };
        private const string DefaultAlgorithm = "SHA1";
        private const string ValidScheme = "otpauth";
        private const string ValidType = "totp";
        private const int DefaultDigits = 6;
        private const int DefaultPeriod = 30;
        private const TotpFormat DefaultFormat = TotpFormat.RFC6238;

        public static KeyUri CreateFromLegacySettings(string[] settings, string secret)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings", "Should not be null.");
            }
            if (settings.Length < 2)
            {
                throw new ArgumentOutOfRangeException("settings", "Should have at least two entries");
            }
            if (secret == null)
            {
                throw new ArgumentOutOfRangeException("secret", "Should not be null.");
            }

            var issuer = "SomeIssuer";
            var digits = settings[1] == "S" ? "5" : settings[1];
            var format = settings[1] == "S" ? TotpFormat.Steam : TotpFormat.RFC6238;
            var period = settings[0];
            var tcurl = (settings.Length > 2) ? settings[2] : null;

            // Construct a uri
            var uri = string.Format("{0}://{1}/{2}:SomeLabel?secret={3}&period={4}&digits={5}&format={6}", ValidScheme, ValidType, Uri.EscapeDataString(issuer), Uri.EscapeDataString(secret), Uri.EscapeDataString(period), Uri.EscapeDataString(digits), format.ToString());
            if (tcurl != null)
            {
                uri += "&timecorrectionurl=" + Uri.EscapeDataString(tcurl);
            }

            return new KeyUri(new Uri(uri));
        }

        public KeyUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri", "Uri should not be null.");
            }
            if (uri.Scheme != ValidScheme)
            {
                throw new ArgumentOutOfRangeException("uri", "Uri scheme must be " + ValidScheme + ".");
            }
            this.Type = EnsureValidType(uri);

            var parsedQuery = ParseQueryString(uri.Query);

            this.Secret = EnsureValidSecret(parsedQuery);
            this.Algorithm = EnsureValidAlgorithm(parsedQuery);
            this.Period = EnsureValidPeriod(parsedQuery);
            this.Digits = EnsureValidDigits(parsedQuery);
            this.TimeCorrectionUrl = EnsureValidTimeCorrectionUrl(parsedQuery);
            this.Format = EnsureValidFormat(parsedQuery);

            EnsureValidLabelAndIssuer(uri, parsedQuery);
        }

        private Uri EnsureValidTimeCorrectionUrl(NameValueCollection query)
        {
            Uri uri;
            if (query.AllKeys.Contains("timecorrectionurl"))
            {
                if (!Uri.TryCreate(query["timecorrectionurl"], UriKind.Absolute, out uri))
                {
                    throw new ArgumentOutOfRangeException("query", "Not a valid timecorrection url");
                }

                if (!uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) && !uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentOutOfRangeException("query", "Time correction urls must start with http:// or https://");
                }

                return uri;
            }

            return null;
        }

        private void EnsureValidLabelAndIssuer(Uri uri, NameValueCollection query)
        {
            var label = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentOutOfRangeException("uri", "No label");
            }

            var labelParts = label.Split(new[] { ':' }, 2);
            if (labelParts.Length == 1)
            {
                this.Issuer = "";
                this.Label = labelParts[0];
            }
            else
            {
                Issuer = labelParts[0];
                Label = labelParts[1];
            }

            Issuer = query["issuer"] ?? Issuer;

            if (string.IsNullOrWhiteSpace(Label))
            {
                throw new ArgumentOutOfRangeException("uri", "No label");
            }
        }

        private static string EnsureValidType(Uri uri)
        {
            if (uri.Host != "totp")
            {
                throw new ArgumentOutOfRangeException("uri", "Only totp is supported.");
            }
            return uri.Host;
        }

        private int EnsureValidDigits(NameValueCollection query)
        {
            int digits = DefaultDigits;
            if (query.AllKeys.Contains("digits") && !int.TryParse(query["digits"], out digits))
            {
                throw new ArgumentOutOfRangeException("query", "Digits not a number");
            }

            return digits;
        }

        private TotpFormat EnsureValidFormat(NameValueCollection query)
        {
            var format = DefaultFormat;
            if (query.AllKeys.Contains("format") && !Enum.TryParse(query["format"], out format))
            {
                throw new ArgumentOutOfRangeException("query", "Not a valid TOTP Format");
            }

            return format;
        }

        private int EnsureValidPeriod(NameValueCollection query)
        {
            int period = DefaultPeriod;
            if (query.AllKeys.Contains("period") && !int.TryParse(query["period"], out period))
            {
                throw new ArgumentOutOfRangeException("query", "Period not a number");
            }

            return period;
        }

        private static string EnsureValidAlgorithm(NameValueCollection query)
        {
            if (query.AllKeys.Contains("algorithm") && !ValidAlgorithms.Contains(query["algorithm"]))
            {
                throw new ArgumentOutOfRangeException("query", "Not a valid algorithm");
            }

            return query["algorithm"] ?? DefaultAlgorithm;
        }

        private static string EnsureValidSecret(NameValueCollection query)
        {
            if (string.IsNullOrWhiteSpace(query["secret"]))
            {
                throw new ArgumentOutOfRangeException("query", "No secret provided.");
            }
            else if (Base32.HasInvalidPadding(query["secret"]))
            {
                throw new ArgumentOutOfRangeException("query", "Secret is not valid base32.");
            }
            else if (!Base32.IsBase32(query["secret"]))
            {
                throw new ArgumentOutOfRangeException("query", "Secret is not valid base32.");
            }

            return query["secret"].TrimEnd('=');
        }

        public string Type { get; set; }
        public string Secret { get; set; }
        public string Algorithm { get; set; }
        public int Digits { get; set; }
        public int Period { get; set; }
        public string Label { get; set; }
        public string Issuer { get; set; }
        public TotpFormat Format { get; set; }
        public Uri TimeCorrectionUrl { get; set; }

        /// <summary>
        /// Naive (and probably buggy) query string parser, but we do not want a dependency on System.Web
        /// </summary>
        private static NameValueCollection ParseQueryString(string queryString)
        {
            var result = new NameValueCollection();
            // remove anything other than query string from url
            queryString = queryString.Substring(queryString.IndexOf('?') + 1);

            foreach (var keyValue in queryString.Split('&'))
            {
                var singlePair = keyValue.Split('=');
                if (singlePair.Length == 2)
                {
                    result.Add(singlePair[0], Uri.UnescapeDataString(singlePair[1]));
                }
            }

            return result;
        }

        internal string ToLegacySettings()
        {
            var format = Digits == 5 ? "S" : Digits.ToString();
            var period = Period.ToString();
            var tc = TimeCorrectionUrl != null ? TimeCorrectionUrl.AbsoluteUri : null;
            var parts = new string[] { format, period, tc };

            return string.Concat(';', parts.Where(c => c != null));
        }

        public Uri GetUri()
        {
            var newQuery = new NameValueCollection();
            if (Period != DefaultPeriod)
            {
                newQuery["period"] = Convert.ToString(Period);
            }
            if (Digits != DefaultDigits)
            {
                newQuery["digits"] = Convert.ToString(Digits);
            }
            if (Algorithm != DefaultAlgorithm)
            {
                newQuery["algorithm"] = Algorithm;
            }
            if (Format != DefaultFormat)
            {
                newQuery["format"] = Format.ToString();
            }
            newQuery["secret"] = Secret;
            newQuery["issuer"] = Issuer;
            if (TimeCorrectionUrl != null)
            {
                newQuery["timecorrectionurl"] = Uri.EscapeDataString(TimeCorrectionUrl.AbsoluteUri);
            }

            var builder = new UriBuilder(ValidScheme, Type)
            {
                Path = "/" + Uri.EscapeDataString(Issuer) + ":" + Uri.EscapeDataString(Label),
                Query = string.Join("&", newQuery.AllKeys.Select(key => key + "=" + newQuery[key]))
            };

            return builder.Uri;
        }
    }
}
