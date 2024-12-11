using UrlboxSDK.Options.Resource;

namespace UrlboxSDK.Options.Validation;

public sealed class UrlboxOptionsValidation
{
    // ** Options that should not be applied if a given option is not set EG FullPage or UseS3 ** //

    /// <summary>
    /// A list of options that can only be used if full_page = true
    /// </summary>
    private static readonly string[] FullPageOptions =
    {
        nameof(UrlboxOptions.FullPageMode),
        nameof(UrlboxOptions.ScrollIncrement),
        nameof(UrlboxOptions.ScrollDelay),
        nameof(UrlboxOptions.DetectFullHeight),
        nameof(UrlboxOptions.MaxSectionHeight),
        nameof(UrlboxOptions.FullWidth)
    };

    /// <summary>
    /// A list of options that can only be used if use_s3 = true
    /// </summary>
    private static readonly string[] S3Options =
    {
        nameof(UrlboxOptions.S3Bucket),
        nameof(UrlboxOptions.S3Path),
        nameof(UrlboxOptions.S3Endpoint),
        nameof(UrlboxOptions.S3Region),
        nameof(UrlboxOptions.S3Storageclass),
        nameof(UrlboxOptions.CdnHost),
    };

    // Define PDF-specific options as a static readonly field
    private static readonly string[] PdfOptions =
    {
        nameof(UrlboxOptions.PdfPageSize),
        nameof(UrlboxOptions.PdfPageRange),
        nameof(UrlboxOptions.PdfPageWidth),
        nameof(UrlboxOptions.PdfPageHeight),
        nameof(UrlboxOptions.PdfMargin),
        nameof(UrlboxOptions.PdfMarginTop),
        nameof(UrlboxOptions.PdfMarginRight),
        nameof(UrlboxOptions.PdfMarginBottom),
        nameof(UrlboxOptions.PdfMarginLeft),
        nameof(UrlboxOptions.PdfAutoCrop),
        nameof(UrlboxOptions.PdfScale),
        nameof(UrlboxOptions.PdfOrientation),
        nameof(UrlboxOptions.PdfBackground),
        nameof(UrlboxOptions.DisableLigatures),
        nameof(UrlboxOptions.Media),
        nameof(UrlboxOptions.Readable),
        nameof(UrlboxOptions.PdfShowHeader),
        nameof(UrlboxOptions.PdfHeader),
        nameof(UrlboxOptions.PdfShowFooter),
        nameof(UrlboxOptions.PdfFooter)
    };

    /// <summary>
    /// Determines if a value is considered "truthy" based on its type, 
    /// including custom Urlbox-specific types.
    /// 
    /// Evaluates as truthy if:
    /// - <see langword="null"/>: Always falsy.
    /// - <see cref="bool"/>: True if true.
    /// - <see cref="int"/> or <see cref="double"/>: True if not zero.
    /// - <see cref="string"/>: True if not empty.
    /// - <see cref="string[]"/>: True if contains elements.
    /// 
    /// Custom types:
    /// - <see cref="SingleToArraySplit"/>: True if String is not empty 
    ///   or StringArray has elements.
    /// - <see cref="BooleanLike"/>: True if any value indicates "true".
    /// - <see cref="NumLike"/>: True if number is non-zero or string not "0".
    /// - <see cref="StrLike"/>: True if not empty or non-zero.
    /// 
    /// Unhandled types are considered truthy.
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

    /// <summary>
    /// Publicly accessible validation method to ensure options are valid.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static UrlboxOptions Validate(UrlboxOptions options)
    {
        ValidateScreenshotOptions(options);
        ValidatePdfOptions(options);
        ValidateFullPageOptions(options);
        ValidateS3Options(options);
        return options;
    }

    /// <summary>
    /// Validates the provided <see cref="UrlboxOptions"/>.
    /// 
    /// Validation Rules:
    /// - If ImgFit is set, either ThumbWidth or ThumbHeight must be specified.
    /// - If ImgPosition is set, ImgFit must also be set.
    /// - If both ImgFit and ImgPosition are set, ImgFit must be "cover" or "contain".
    /// 
    /// Throws:
    /// - <see cref="ArgumentException"/> if validation rule is violated.
    /// 
    /// Returns:
    /// - The validated <see cref="UrlboxOptions"/> object if all checks pass.
    /// </summary>
    /// <param name="options">The <see cref="UrlboxOptions"/> instance to validate.</param>
    /// <returns>The validated <see cref="UrlboxOptions"/> instance.</returns>
    private static UrlboxOptions ValidateScreenshotOptions(UrlboxOptions options)
    {
        var thumbSizes = options.ThumbWidth != null || options.ThumbHeight != null;
        bool hasImgFit = options.ImgFit != null && Enum.IsDefined(typeof(ImgFit), options.ImgFit);
        bool hasImgPosition = options.ImgPosition != null && Enum.IsDefined(typeof(ImgPosition), options.ImgPosition);
        var imgFitIsCoverOrContain = options.ImgFit == UrlboxSDK.Options.Resource.ImgFit.Cover || options.ImgFit == UrlboxSDK.Options.Resource.ImgFit.Contain;

        if (!thumbSizes && hasImgFit)
        {
            throw new ArgumentException("Invalid Configuration: Image Fit is included despite ThumbWidth nor ThumbHeight being set.");
        }

        if (!hasImgFit && hasImgPosition)
        {
            throw new ArgumentException("Invalid Configuration: Image Position is included despite Image Fit not being set.");
        }

        if (hasImgFit && hasImgPosition && !imgFitIsCoverOrContain)
        {
            throw new ArgumentException("Invalid Configuration: Image Position is included despite Image Fit not being set to 'cover' or 'contain'.");
        }

        return options;
    }

    /// <summary>
    /// Validates the provided <see cref="UrlboxOptions"/>.
    /// 
    /// Validation Rules:
    /// - If FullPage is not set to true, no full-page-specific options should be included.
    /// 
    /// Throws:
    /// - <see cref="ArgumentException"/> if full-page options are set when FullPage is false or not set.
    /// 
    /// Returns:
    /// - The validated <see cref="UrlboxOptions"/> instance.
    /// </summary>
    /// <param name="options">The <see cref="UrlboxOptions"/> instance to validate.</param>
    /// <returns>The validated <see cref="UrlboxOptions"/> instance.</returns>
    private static UrlboxOptions ValidateFullPageOptions(UrlboxOptions options)
    {
        bool isNotFullPage = !options.FullPage.HasValue || (options.FullPage.HasValue && options.FullPage.Value.Bool != true);
        bool hasFullPageOptions = HasOptionsInCategory(FullPageOptions, options);
        if (
            isNotFullPage && hasFullPageOptions
        )
        {
            throw new ArgumentException("Invalid configuration: Full-page options are included despite 'FullPage' being set to false.");
        }
        return options;
    }

    /// <summary>
    /// Validates the provided <see cref="UrlboxOptions"/>.
    /// 
    /// Validation Rules:
    /// - If UseS3 is not set to true, no S3-specific options should be included.
    /// 
    /// Throws:
    /// - <see cref="ArgumentException"/> if S3 options are set when UseS3 is false or not set.
    /// 
    /// Returns:
    /// - The validated <see cref="UrlboxOptions"/> instance.
    /// </summary>
    /// <param name="options">The <see cref="UrlboxOptions"/> instance to validate.</param>
    /// <returns>The validated <see cref="UrlboxOptions"/> instance.</returns>
    private static UrlboxOptions ValidateS3Options(UrlboxOptions options)
    {
        bool isNotUsingS3 = !options.UseS3.HasValue || (options.UseS3.HasValue && options.UseS3.Value.Bool != true);
        bool hasS3Options = HasOptionsInCategory(S3Options, options);
        if (isNotUsingS3 && hasS3Options)
        {
            throw new ArgumentException("Invalid configuration: S3 options are included despite 'UseS3' being set to false.");
        }
        return options;
    }

    /// <summary>
    /// Validates the provided <see cref="UrlboxOptions"/>.
    /// 
    /// Validation Rules:
    /// - If Format is not set to Pdf, no PDF-specific options should be included.
    /// 
    /// Throws:
    /// - <see cref="ArgumentException"/> if PDF options are set when Format is not Pdf.
    /// 
    /// Returns:
    /// - The validated <see cref="UrlboxOptions"/> instance.
    /// </summary>
    /// <param name="options">The <see cref="UrlboxOptions"/> instance to validate.</param>
    /// <returns>The validated <see cref="UrlboxOptions"/> instance.</returns>
    private static UrlboxOptions ValidatePdfOptions(UrlboxOptions options)
    {
        bool isNotUsingPdf = options.Format != UrlboxSDK.Options.Resource.Format.Pdf;
        bool hasPdfOptions = HasOptionsInCategory(PdfOptions, options);
        if (isNotUsingPdf && hasPdfOptions)
        {
            throw new ArgumentException("One or more PDF-specific options are only valid for the PDF format.");
        }

        return options;
    }

    /// <summary>
    /// Determines if any properties in the specified category are set in the given options.
    /// </summary>
    /// <param name="category">Array of property names to check within the options.</param>
    /// <param name="options">The options object to inspect.</param>
    /// <returns>True if any property in the category is set; otherwise, false.</returns>
    private static bool HasOptionsInCategory(string[] category, UrlboxOptions options)
    {
        return category
         .Any(propertyName =>
         {
             var property = options.GetType().GetProperty(propertyName);
             if (property == null) return false;
             var value = property.GetValue(options);
             return UrlboxOptionsValidation.IsNullOption(value);
         });
    }
}
