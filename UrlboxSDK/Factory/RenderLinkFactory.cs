using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UrlboxSDK.Options.Resource;
using UrlboxSDK.Options.Validation;

namespace UrlboxSDK.Factory;

/// <summary>
/// A class encompassing render link generation logic.
/// </summary>
sealed class RenderLinkFactory
{
    private readonly string key;
    private readonly string secret;

    public RenderLinkFactory(string key, string secret)
    {
        this.key = key;
        this.secret = secret;
    }

    /// <summary>
    /// Turns an instance of UrlboxOptions into a URL query string.
    /// </summary>
    /// <param name="options"></param>
    /// <returns>A string with a formed query based on the options.</returns>
    private static string ToQueryString(UrlboxOptions options)
    {
        // Filter by reflection class' props
        PropertyInfo[] properties = options.GetType().GetProperties();
        string[] result = properties
            .Where(prop =>
                {
                    // Filter out falsy values
                    object? value = prop.GetValue(options, null);
                    return UrlboxOptionsValidation.IsNullOption(value);
                })
            .OrderBy(prop => prop.Name)
            // Convert not null values to string representation
            .Select(prop =>
            {
                object? propValue = prop.GetValue(options) ??
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
        if (string.IsNullOrEmpty(input))
        {
            return input; // Return as-is if input is null or empty
        }

        return input switch
        {
            "FailOn5Xx" => "fail_on_5xx",
            "FailOn4Xx" => "fail_on_4xx",
            "Highlightfg" => "highlight_fg",
            "Highlightbg" => "highlight_bg",
            "S3Storageclass" => "s3_storage_class",
            _ => ConvertToSnakeCase(input)
        };
    }

    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>The snake_case representation of the string.</returns>
    private static string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        StringBuilder result = new();

        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];
            char? previousChar = i > 0 ? input[i - 1] : (char?)null;

            // Add an underscore before an uppercase letter when:
            // - It's not the first character
            // - The previous character is not an underscore
            // - The previous character is not uppercase
            if (i > 0 &&
                char.IsUpper(currentChar) &&
                previousChar.HasValue &&
                previousChar != '_' &&
                !char.IsUpper(previousChar.Value))
            {
                result.Append('_');
            }

            result.Append(currentChar);
        }

        return result.ToString().ToLower();
    }

    /// <summary>
    /// Converts object types to string representations, including the custom Urlbox types.
    /// Throws exception if the value is null or cannot be converted.
    /// </summary>
    /// <param name="value">The object to convert to a string.</param>
    /// <returns>A string rep of the provided object.</returns>
    private static string ConvertToString(object value)
    {
        if (!UrlboxOptionsValidation.IsNullOption(value))
        {
            throw new System.Exception("Value contains no valid content.");
        }

        return value switch
        {
            string[] stringArray => string.Join(",", stringArray),
            Enum enumValue => enumValue.ToString().ToLower(),
            bool boolValue => boolValue.ToString().ToLower(),
            // Default case: Convert all other types using Convert.ToString
            _ => Convert.ToString(value)
                ?? throw new System.Exception("Could not convert value to string.")
        };
    }

    /// <summary>
    /// Generates a Urlbox render link.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="format"></param>
    /// <returns>The Urlbox Render Link</returns>
    public string GenerateRenderLink(string baseUrl, UrlboxOptions options, bool sign = true)
    {
        // Either the options.Format or PNG as default
        string format = options.Format?.ToString().ToLower() ?? "png";

        string queryString = ToQueryString(options);
        if (sign)
        {
            return string.Format(
                baseUrl + "/v1/{0}/{1}/{2}?{3}",
                key,
                GenerateToken(queryString),
                format,
                queryString
            );
        }
        else
        {
            return string.Format(
                baseUrl + "/v1/{0}/{1}?{2}",
                key,
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
    private string GenerateToken(string queryString)
    {
        HMACSHA1 sha = new(Encoding.UTF8.GetBytes(secret));
        MemoryStream stream = new(Encoding.UTF8.GetBytes(queryString));
        return sha.ComputeHash(stream).Aggregate("", (current, next) => current + string.Format("{0:x2}", next), current => current);
    }
}
