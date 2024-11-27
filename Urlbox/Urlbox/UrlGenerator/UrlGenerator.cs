using System.Collections.Generic;
using System.Security.Cryptography;

namespace UrlboxSDK;
/// <summary>
/// A class encompassing Url Generation logic.
/// </summary>
public sealed class UrlGenerator
{
    private readonly String key;
    private readonly String secret;

    public UrlGenerator(string key, string secret)
    {
        this.key = key;
        this.secret = secret;
    }

    /// <summary>
    /// Turns an instance of UrlboxOptions into a URL query string.
    /// </summary>
    /// <param name="options"></param>
    /// <returns>A string with a formed query based on the options.</returns>
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
                        !(value is bool valueBool && valueBool == false) && // skip false if bool
                        !(value is int valueInt && valueInt == 0) && // skip 0's if int
                        !(value is double valueDouble && valueDouble == 0.0) && // skip 0's if double
                        !(value is string valueString && string.IsNullOrEmpty(valueString)) && // skip empty strings if string
                        !(value is string[] valueArray && valueArray.Length == 0); // skip empty arrays
                })
            .OrderBy(prop => prop.Name)
            // Convert not null values to string representation
            .Select(prop =>
            {
                var propValue = prop.GetValue(options) ??
                    throw new ArgumentException($"Cannot convert options to a query string: trying to convert {prop.Name} which has a null value.");
                string stringValue = ConvertToString(propValue);
                return new KeyValuePair<string, string>(prop.Name, stringValue);
            })
            .Where(pair => !pair.Key.ToLower().Equals("format")) // Skip 'format' if present
            .Select(pair => string.Format("{0}={1}", FormatKeyName(pair.Key), Uri.EscapeDataString(pair.Value)))
            .ToArray();

        return string.Join("&", result);
    }

    /// <summary>
    /// Formats an input to snake_case
    /// </summary>
    /// <param name="input"></param>
    /// <returns>The snake_case variant of the string input</returns>
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
        if (result == null)
        {
            // TODO Test for this
            throw new Exception("Could not convert value to string.");
        }
        else
        {
            if (result.Equals("False") || result.Equals("True"))
            {
                result = result.ToLower();
            }
            return result;
        }
    }

    /// <summary>
    /// Generates a Urlbox render link.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="format"></param>
    /// <returns>The Urlbox Render Link</returns>
    public string GenerateRenderLink(UrlboxOptions options, string format = "png", bool sign = false)
    {
        var queryString = ToQueryString(options);
        if (sign)
        {
            return string.Format(Urlbox.BASE_URL + "/v1/{0}/{1}/{2}?{3}",
                                 this.key,
                                 generateToken(queryString),
                                 format,
                                 queryString
                                 );
        }
        else
        {
            return string.Format(Urlbox.BASE_URL + "/v1/{0}/{1}?{2}",
                                             this.key,
                                             format,
                                             queryString
                                             );
        }
    }

    /// <summary>
    /// Generates a signed variant of one's secret Urlbox token.
    /// </summary>
    /// <param name="queryString"></param>
    /// <returns>The signed token</returns>
    private string generateToken(string queryString)
    {
        HMACSHA1 sha = new HMACSHA1(Encoding.UTF8.GetBytes(this.secret));
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(queryString));
        return sha.ComputeHash(stream).Aggregate("", (current, next) => current + String.Format("{0:x2}", next), current => current);
    }
}
