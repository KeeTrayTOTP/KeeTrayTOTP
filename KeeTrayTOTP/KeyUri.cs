using KeeTrayTOTP.Libraries;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace KeeTrayTOTP
{
    public class KeyUri
    {
        private static string[] ValidAlgorithms = new[] { "SHA1", "SHA256", "SHA512" };
        private const string DefaultAlgorithm = "SHA1";
        private const string ValidScheme = "otpauth";
        private const int DefaultDigits = 6;
        private const int DefaultPeriod = 30;

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
            this.Digits = EnsureValidDigits(parsedQuery);
            this.Period = EnsureValidPeriod(parsedQuery);

            EnsureValidLabelAndIssuer(uri, parsedQuery);
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

        public Uri GetUri()
        {
            var newQuery = new NameValueCollection();
            if (Period != 30)
            {
                newQuery["period"] = Convert.ToString(Period);
            }
            if (Digits != 6)
            {
                newQuery["digits"] = Convert.ToString(Digits);
            }
            if (Algorithm != "SHA1")
            {
                newQuery["algorithm"] = Algorithm;
            }
            newQuery["secret"] = Secret;
            newQuery["issuer"] = Issuer;

            var builder = new UriBuilder(ValidScheme, Type)
            {
                Path = "/" + Uri.EscapeDataString(Issuer) + ":" + Uri.EscapeDataString(Label),
                Query = string.Join("&", newQuery.AllKeys.Select(key => key + "=" + newQuery[key]))
            };

            return builder.Uri;
        }
    }
}
