using System.Collections.Generic;
using System.Security.Cryptography;
using UrlboxSDK.Options.Resource;

namespace UrlboxSDK.Options.Validation;

/// <summary>
/// </summary>
public sealed class UrlboxOptionsValidation
{
    /// <summary>
    /// Checks a value based on its type for falsy, including custom Urlbox Definitions
    /// </summary>
    public static bool IsNullOption(object? value)
    {
        return value switch
        {
            // Filter out non-custom falsy values
            null => false,
            bool valueBool => valueBool, // Include only if true
            int valueInt => valueInt != 0, // Include only if non-zero
            double valueDouble => Math.Abs(valueDouble) >= double.Epsilon, // Include only if non-zero
            string valueString => !string.IsNullOrEmpty(valueString), // Include only if not empty
            string[] valueArray => valueArray.Length > 0, // Include only if array has elements
                                                          // Filter out falsey custom value types
            SingleToArraySplit singleToArraySplit =>
                !string.IsNullOrEmpty(singleToArraySplit.String) && singleToArraySplit.String != "" ||
                (singleToArraySplit.StringArray != null && singleToArraySplit.StringArray.Length > 0),
            BooleanLike booleanLike =>
                booleanLike.Bool.HasValue && booleanLike.Bool != false ||
                booleanLike.Double.HasValue && booleanLike.Double != 0 ||
                !string.IsNullOrEmpty(booleanLike.String) && booleanLike.String != "false",
            NumLike numLike =>
                numLike.Integer.HasValue && numLike.Integer != 0 ||
                !string.IsNullOrEmpty(numLike.String) && numLike.String != "0",
            StrLike strLike =>
                strLike.Double.HasValue && strLike.Double != 0 ||
                !string.IsNullOrEmpty(strLike.String),
            _ => true // Include all other non-handled types
        };
    }
}
