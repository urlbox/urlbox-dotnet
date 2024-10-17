using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Screenshots
{
    public class UrlGenerator
    {
        private String key;
        private String secret;

        public UrlGenerator(string key, string secret)
        {
            this.key = key;
            this.secret = secret;
        }

        // private string ToQueryString(IDictionary<string, object> options)
        // {
        //     var result = options
        //         .ToList()
        //         .Where(pair => !pair.Key.ToLower().Equals("format")) // skip format option if present
        //         .Select(pair => new KeyValuePair<string, string>(pair.Key, ConvertToString(pair.Value))) // convert values to string
        //         .Where(pair => !String.IsNullOrEmpty(pair.Value)) // skip empty/null values
        //         .Select(pair => string.Format("{0}={1}", FormatKeyName(pair.Key), Uri.EscapeDataString(pair.Value)))
        //         .ToArray();
        //     return String.Join("&", result);
        // }

        private string ToQueryString(UrlboxOptions options)
        {
            // Filter by reflection class' props
            var properties = options.GetType().GetProperties();
            var result = properties
                .Where(prop =>
                // Filter out falsy values
                    {
                        var value = prop.GetValue(options, null);
                        return value != null &&
                            !(value is bool && (bool)value == false) && // skip false if bool
                            !(value is int && (int)value == 0) && // skip 0's if int
                            !(value is double && (double)value == 0.0) && // skip 0's if double
                            !(value is string && string.IsNullOrEmpty((string)value)) && // skip empty strings if string
                            !(value is string[] arr && arr.Length == 0); // skip empty arrays
                    })
                // Convert values to string representations
                .Select(prop => new KeyValuePair<string, string>(prop.Name, ConvertToString(prop.GetValue(options))))
                .Where(pair => !pair.Key.ToLower().Equals("format")) // Skip 'format' if present
                .Select(pair => string.Format("{0}={1}", FormatKeyName(pair.Key), Uri.EscapeDataString(pair.Value)))
                .ToArray();

            return string.Join("&", result);
        }

        private static string FormatKeyName(string input)
        {
            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) && !input[i - 1].Equals('_') ? "_" + x.ToString() : x.ToString())).ToLower();

        }

        /// <summary>
        /// Converts the object to a string. If the object is a string array,
        /// it formats the array as a comma-separated string.
        /// </summary>
        /// <param name="value">The object to convert to a string. Can be a string array or a boolean value.</param>
        /// <returns>
        /// A string representation of the provided object.
        /// </returns>
        private static string ConvertToString(object value)
        {
            if (value is string[] stringArray)
            {
                return $"{string.Join(",", stringArray)}";
            }

            var result = Convert.ToString(value);
            if (result.Equals("False") || result.Equals("True"))
            {
                result = result.ToLower();
            }
            return result;
        }


        // public string GenerateUrlboxUrl(IDictionary<string, object> options, string format = "png")
        // {
        //     var qs = ToQueryString(options);
        //     return string.Format("https://api.urlbox.com/v1/{0}/{1}/{2}?{3}",
        //                          this.key,
        //                          generateToken(qs),
        //                          format,
        //                          qs
        //                          );
        // }

        public string GenerateUrlboxUrl(UrlboxOptions options, string format = "png")
        {
            var qs = ToQueryString(options);
            return string.Format("https://api.urlbox.com/v1/{0}/{1}/{2}?{3}",
                                 this.key,
                                 generateToken(qs),
                                 format,
                                 qs
                                 );
        }


        private string generateToken(string queryString)
        {
            HMACSHA1 sha = new HMACSHA1(Encoding.UTF8.GetBytes(this.secret));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(queryString));
            return sha.ComputeHash(stream).Aggregate("", (current, next) => current + String.Format("{0:x2}", next), current => current);
        }
    }
}
