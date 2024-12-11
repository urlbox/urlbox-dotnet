using UrlboxSDK.Options.Resource;
using UrlboxSDK.Options.Validation;

namespace UrlboxSDK.Options.Builder;

public sealed class UrlboxOptionsBuilder
{
    private readonly UrlboxOptions _options;

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
    /// Constructor
    /// </summary>
    /// <param name="url"></param>
    /// <param name="html"></param>
    public UrlboxOptionsBuilder(string? url = null, string? html = null)
    {
        _options = new UrlboxOptions(
            url,
            html
        );
    }

    /// <summary>
    /// Builds the UrlboxOptions instance after validating.
    /// </summary>
    /// <returns></returns>
    public UrlboxOptions Build()
    {
        return Validate(_options);
    }

    private static UrlboxOptions Validate(UrlboxOptions options)
    {
        ValidateScreenshotOptions(options);
        ValidatePdfOptions(options);
        ValidateFullPageOptions(options);
        ValidateS3Options(options);
        return options;
    }

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

    public UrlboxOptionsBuilder WebhookUrl(string webhookUrl)
    {
        _options.WebhookUrl = webhookUrl;
        return this;
    }

    public UrlboxOptionsBuilder Format(Format format)
    {
        _options.Format = format;
        return this;
    }

    public UrlboxOptionsBuilder Width(int width)
    {
        _options.Width = width;
        return this;
    }

    public UrlboxOptionsBuilder Height(int height)
    {
        _options.Height = height;
        return this;
    }

    public UrlboxOptionsBuilder FullPage()
    {
        _options.FullPage = true;
        return this;
    }

    public UrlboxOptionsBuilder Selector(string selector)
    {
        _options.Selector = selector;
        return this;
    }

    public UrlboxOptionsBuilder Clip(string clip)
    {
        _options.Clip = clip;
        return this;
    }

    public UrlboxOptionsBuilder Gpu()
    {
        _options.Gpu = true;
        return this;
    }

    public UrlboxOptionsBuilder ResponseType(ResponseType responseType)
    {
        _options.ResponseType = responseType;
        return this;
    }

    public UrlboxOptionsBuilder BlockAds()
    {
        _options.BlockAds = true;
        return this;
    }

    public UrlboxOptionsBuilder HideCookieBanners()
    {
        _options.HideCookieBanners = true;
        return this;
    }

    public UrlboxOptionsBuilder ClickAccept()
    {
        _options.ClickAccept = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockUrls(params string[] blockUrls)
    {
        _options.BlockUrls = blockUrls;
        return this;
    }

    public UrlboxOptionsBuilder BlockImages()
    {
        _options.BlockImages = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockFonts()
    {
        _options.BlockFonts = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockMedias()
    {
        _options.BlockMedias = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockStyles()
    {
        _options.BlockStyles = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockScripts()
    {
        _options.BlockScripts = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockFrames()
    {
        _options.BlockFrames = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockFetch()
    {
        _options.BlockFetch = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockXhr()
    {
        _options.BlockXhr = true;
        return this;
    }

    public UrlboxOptionsBuilder BlockSockets()
    {
        _options.BlockSockets = true;
        return this;
    }

    public UrlboxOptionsBuilder HideSelector(string hideSelector)
    {
        _options.HideSelector = hideSelector;
        return this;
    }

    public UrlboxOptionsBuilder Js(string js)
    {
        _options.Js = js;
        return this;
    }

    public UrlboxOptionsBuilder Css(string css)
    {
        _options.Css = css;
        return this;
    }

    public UrlboxOptionsBuilder DarkMode()
    {
        _options.DarkMode = true;
        return this;
    }

    public UrlboxOptionsBuilder ReducedMotion()
    {
        _options.ReducedMotion = true;
        return this;
    }

    public UrlboxOptionsBuilder Retina()
    {
        _options.Retina = true;
        return this;
    }

    public UrlboxOptionsBuilder ThumbWidth(int thumbWidth)
    {
        _options.ThumbWidth = thumbWidth;
        return this;
    }

    public UrlboxOptionsBuilder ThumbHeight(int thumbHeight)
    {
        _options.ThumbHeight = thumbHeight;
        return this;
    }

    public UrlboxOptionsBuilder ImgFit(ImgFit imgFit)
    {
        _options.ImgFit = imgFit;
        return this;
    }

    public UrlboxOptionsBuilder ImgPosition(ImgPosition imgPosition)
    {
        _options.ImgPosition = imgPosition;
        return this;
    }

    public UrlboxOptionsBuilder ImgBg(string imgBg)
    {
        _options.ImgBg = imgBg;
        return this;
    }

    public UrlboxOptionsBuilder ImgPad(string imgPad)
    {
        _options.ImgPad = imgPad;
        return this;
    }

    public UrlboxOptionsBuilder Quality(int quality)
    {
        _options.Quality = quality;
        return this;
    }

    public UrlboxOptionsBuilder Transparent()
    {
        _options.Transparent = true;
        return this;
    }

    public UrlboxOptionsBuilder MaxHeight(int maxHeight)
    {
        _options.MaxHeight = maxHeight;
        return this;
    }

    public UrlboxOptionsBuilder Download(string download)
    {
        _options.Download = download;
        return this;
    }

    public UrlboxOptionsBuilder PdfPageSize(PdfPageSize pdfPageSize)
    {
        _options.PdfPageSize = pdfPageSize;
        return this;
    }

    public UrlboxOptionsBuilder PdfPageRange(string pdfPageRange)
    {
        _options.PdfPageRange = pdfPageRange;
        return this;
    }

    public UrlboxOptionsBuilder PdfPageWidth(int pdfPageWidth)
    {
        _options.PdfPageWidth = pdfPageWidth;
        return this;
    }

    public UrlboxOptionsBuilder PdfPageHeight(int pdfPageHeight)
    {
        _options.PdfPageHeight = pdfPageHeight;
        return this;
    }

    public UrlboxOptionsBuilder PdfMargin(PdfMargin pdfMargin)
    {
        _options.PdfMargin = pdfMargin;
        return this;
    }

    public UrlboxOptionsBuilder PdfMarginTop(int pdfMarginTop)
    {
        _options.PdfMarginTop = pdfMarginTop;
        return this;
    }

    public UrlboxOptionsBuilder PdfMarginRight(int pdfMarginRight)
    {
        _options.PdfMarginRight = pdfMarginRight;
        return this;
    }

    public UrlboxOptionsBuilder PdfMarginBottom(int pdfMarginBottom)
    {
        _options.PdfMarginBottom = pdfMarginBottom;
        return this;
    }

    public UrlboxOptionsBuilder PdfMarginLeft(int pdfMarginLeft)
    {
        _options.PdfMarginLeft = pdfMarginLeft;
        return this;
    }

    public UrlboxOptionsBuilder PdfAutoCrop()
    {
        _options.PdfAutoCrop = true;
        return this;
    }

    public UrlboxOptionsBuilder PdfScale(double pdfScale)
    {
        _options.PdfScale = pdfScale;
        return this;
    }

    public UrlboxOptionsBuilder PdfOrientation(PdfOrientation pdfOrientation)
    {
        _options.PdfOrientation = pdfOrientation;
        return this;
    }

    public UrlboxOptionsBuilder PdfBackground()
    {
        _options.PdfBackground = true;
        return this;
    }

    public UrlboxOptionsBuilder DisableLigatures()
    {
        _options.DisableLigatures = true;
        return this;
    }

    public UrlboxOptionsBuilder Media(Media media)
    {
        _options.Media = media;
        return this;
    }

    public UrlboxOptionsBuilder PdfShowHeader()
    {
        _options.PdfShowHeader = true;
        return this;
    }

    public UrlboxOptionsBuilder PdfHeader(string pdfHeader)
    {
        _options.PdfHeader = pdfHeader;
        return this;
    }

    public UrlboxOptionsBuilder PdfShowFooter()
    {
        _options.PdfShowFooter = true;
        return this;
    }

    public UrlboxOptionsBuilder PdfFooter(string pdfFooter)
    {
        _options.PdfFooter = pdfFooter;
        return this;
    }

    public UrlboxOptionsBuilder Readable()
    {
        _options.Readable = true;
        return this;
    }

    public UrlboxOptionsBuilder Force()
    {
        _options.Force = true;
        return this;
    }

    public UrlboxOptionsBuilder Unique(string unique)
    {
        _options.Unique = unique;
        return this;
    }

    public UrlboxOptionsBuilder Ttl(int ttl)
    {
        _options.Ttl = ttl;
        return this;
    }

    public UrlboxOptionsBuilder Proxy(string proxy)
    {
        _options.Proxy = proxy;
        return this;
    }

    /// <summary>
    /// Tightens a type to string or string[] for Urlbox options which allow singles+multiples
    /// </summary>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static SingleToArraySplit ValidateStringOrArray(object value, string propertyName)
    {
        SingleToArraySplit splitValue = new();
        return value switch
        {
            string stringValue => splitValue.String = stringValue,
            string[] stringArrayValue => splitValue.StringArray = stringArrayValue,
            _ => throw new ArgumentException($"{propertyName} must be either a string or a string array.")
        };
    }

    public UrlboxOptionsBuilder Header(object header)
    {
        _options.Header = ValidateStringOrArray(header, nameof(Header));
        return this;
    }

    public UrlboxOptionsBuilder Cookie(object cookie)
    {
        _options.Cookie = ValidateStringOrArray(cookie, nameof(Cookie));
        return this;
    }

    public UrlboxOptionsBuilder UserAgent(string userAgent)
    {
        _options.UserAgent = userAgent;
        return this;
    }

    public UrlboxOptionsBuilder Platform(string platform)
    {
        _options.Platform = platform;
        return this;
    }

    public UrlboxOptionsBuilder AcceptLang(string acceptLang)
    {
        _options.AcceptLang = acceptLang;
        return this;
    }

    public UrlboxOptionsBuilder Authorization(string authorization)
    {
        _options.Authorization = authorization;
        return this;
    }

    public UrlboxOptionsBuilder Tz(string tz)
    {
        _options.Tz = tz;
        return this;
    }

    public UrlboxOptionsBuilder EngineVersion(EngineVersion engineVersion)
    {
        _options.EngineVersion = engineVersion;
        return this;
    }

    public UrlboxOptionsBuilder Delay(int delay)
    {
        _options.Delay = delay;
        return this;
    }

    public UrlboxOptionsBuilder Timeout(int timeout)
    {
        _options.Timeout = timeout;
        return this;
    }

    public UrlboxOptionsBuilder WaitUntil(WaitUntil waitUntil)
    {
        _options.WaitUntil = waitUntil;
        return this;
    }

    public UrlboxOptionsBuilder WaitFor(string waitFor)
    {
        _options.WaitFor = waitFor;
        return this;
    }

    public UrlboxOptionsBuilder WaitToLeave(string waitToLeave)
    {
        _options.WaitToLeave = waitToLeave;
        return this;
    }

    public UrlboxOptionsBuilder WaitTimeout(int waitTimeout)
    {
        _options.WaitTimeout = waitTimeout;
        return this;
    }

    public UrlboxOptionsBuilder FailIfSelectorMissing()
    {
        _options.FailIfSelectorMissing = true;
        return this;
    }

    public UrlboxOptionsBuilder FailIfSelectorPresent()
    {
        _options.FailIfSelectorPresent = true;
        return this;
    }

    public UrlboxOptionsBuilder FailOn4xx()
    {
        _options.FailOn4Xx = true;
        return this;
    }

    public UrlboxOptionsBuilder FailOn5xx()
    {
        _options.FailOn5Xx = true;
        return this;
    }

    public UrlboxOptionsBuilder ScrollTo(string scrollTo)
    {
        _options.ScrollTo = scrollTo;
        return this;
    }

    public UrlboxOptionsBuilder Click(string click)
    {
        _options.Click = click;
        return this;
    }

    public UrlboxOptionsBuilder ClickAll(string clickAll)
    {
        _options.ClickAll = clickAll;
        return this;
    }

    public UrlboxOptionsBuilder Hover(string hover)
    {
        _options.Hover = hover;
        return this;
    }

    public UrlboxOptionsBuilder BgColor(string bgColor)
    {
        _options.BgColor = bgColor;
        return this;
    }

    public UrlboxOptionsBuilder DisableJs()
    {
        _options.DisableJs = true;
        return this;
    }

    public UrlboxOptionsBuilder FullPageMode(FullPageMode fullPageMode)
    {
        _options.FullPageMode = fullPageMode;
        return this;
    }

    public UrlboxOptionsBuilder FullWidth()
    {
        _options.FullWidth = true;
        return this;
    }

    public UrlboxOptionsBuilder AllowInfinite()
    {
        _options.AllowInfinite = true;
        return this;
    }

    public UrlboxOptionsBuilder SkipScroll()
    {
        _options.SkipScroll = true;
        return this;
    }

    public UrlboxOptionsBuilder DetectFullHeight()
    {
        _options.DetectFullHeight = true;
        return this;
    }

    public UrlboxOptionsBuilder MaxSectionHeight(int maxSectionHeight)
    {
        _options.MaxSectionHeight = maxSectionHeight;
        return this;
    }

    public UrlboxOptionsBuilder ScrollIncrement(int scrollIncrement)
    {
        _options.ScrollIncrement = scrollIncrement;
        return this;
    }

    public UrlboxOptionsBuilder ScrollDelay(int scrollDelay)
    {
        _options.ScrollDelay = scrollDelay;
        return this;
    }

    public UrlboxOptionsBuilder Highlight(string highlight)
    {
        _options.Highlight = highlight;
        return this;
    }

    public UrlboxOptionsBuilder Highlightfg(string Highlightfg)
    {
        _options.Highlightfg = Highlightfg;
        return this;
    }

    public UrlboxOptionsBuilder Highlightbg(string Highlightbg)
    {
        _options.Highlightbg = Highlightbg;
        return this;
    }

    public UrlboxOptionsBuilder Latitude(double latitude)
    {
        StrLike latitudeStrLike = new()
        {
            Double = latitude
        };
        _options.Latitude = latitudeStrLike;
        return this;
    }

    public UrlboxOptionsBuilder Longitude(double longitude)
    {
        StrLike longitudeStrLike = new()
        {
            Double = longitude
        };
        _options.Longitude = longitudeStrLike;
        return this;
    }

    public UrlboxOptionsBuilder Accuracy(int accuracy)
    {
        _options.Accuracy = accuracy;
        return this;
    }

    public UrlboxOptionsBuilder UseS3()
    {
        _options.UseS3 = true;
        return this;
    }

    public UrlboxOptionsBuilder S3Path(string s3Path)
    {
        _options.S3Path = s3Path;
        return this;
    }

    public UrlboxOptionsBuilder S3Bucket(string s3Bucket)
    {
        _options.S3Bucket = s3Bucket;
        return this;
    }

    public UrlboxOptionsBuilder S3Endpoint(string s3Endpoint)
    {
        _options.S3Endpoint = s3Endpoint;
        return this;
    }

    public UrlboxOptionsBuilder S3Region(string s3Region)
    {
        _options.S3Region = s3Region;
        return this;
    }

    public UrlboxOptionsBuilder CdnHost(string cdnHost)
    {
        _options.CdnHost = cdnHost;
        return this;
    }

    public UrlboxOptionsBuilder S3Storageclass(S3Storageclass s3Storageclass)
    {
        _options.S3Storageclass = s3Storageclass;
        return this;
    }

    public UrlboxOptionsBuilder SaveHtml()
    {
        _options.SaveHtml = true;
        return this;
    }

    public UrlboxOptionsBuilder SaveMhtml()
    {
        _options.SaveMhtml = true;
        return this;
    }

    public UrlboxOptionsBuilder SaveMarkdown()
    {
        _options.SaveMarkdown = true;
        return this;
    }

    public UrlboxOptionsBuilder SaveMetadata()
    {
        _options.SaveMetadata = true;
        return this;
    }

    public UrlboxOptionsBuilder Metadata()
    {
        _options.Metadata = true;
        return this;
    }
}
