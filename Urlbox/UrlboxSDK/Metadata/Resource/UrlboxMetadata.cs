namespace UrlboxSDK;

/// <summary>
/// Represents Metadata for a Urlbox Response when save_metadata or metadata options are set to true
/// </summary>
public sealed class UrlboxMetadata
{
    public string UrlRequested { get; }
    public string UrlResolved { get; }
    public string Url { get; }
    public string? Author { get; }
    public string? Date { get; }
    public string? Description { get; }
    public string? Image { get; }
    public string? Logo { get; }
    public string? Publisher { get; }
    public string? Title { get; }
    public string? OgTitle { get; }
    public OgImage[]? OgImage { get; }
    public string? OgDescription { get; }
    public string? OgUrl { get; }
    public string? OgType { get; }
    public string? OgSiteName { get; }
    public string? OgLocale { get; }
    public string? Charset { get; }
    public string? TwitterCard { get; }
    public string? TwitterSite { get; }
    public string? TwitterCreator { get; }

    public UrlboxMetadata(
        string urlRequested,
        string urlResolved,
        string url,
        string? author = null,
        string? date = null,
        string? description = null,
        string? image = null,
        string? logo = null,
        string? publisher = null,
        string? title = null,
        string? ogTitle = null,
        OgImage[]? ogImage = null,
        string? ogDescription = null,
        string? ogUrl = null,
        string? ogType = null,
        string? ogSiteName = null,
        string? ogLocale = null,
        string? charset = null,
        string? twitterCard = null,
        string? twitterSite = null,
        string? twitterCreator = null
    )
    {
        UrlRequested = urlRequested ?? throw new ArgumentNullException(nameof(urlRequested));
        UrlResolved = urlResolved ?? throw new ArgumentNullException(nameof(urlResolved));
        Url = url ?? throw new ArgumentNullException(nameof(url));

        if (author != null) Author = author;
        if (date != null) Date = date;
        if (description != null) Description = description;
        if (image != null) Image = image;
        if (logo != null) Logo = logo;
        if (publisher != null) Publisher = publisher;
        if (title != null) Title = title;
        if (ogTitle != null) OgTitle = ogTitle;
        if (ogImage != null) OgImage = ogImage;
        if (ogDescription != null) OgDescription = ogDescription;
        if (ogUrl != null) OgUrl = ogUrl;
        if (ogType != null) OgType = ogType;
        if (ogSiteName != null) OgSiteName = ogSiteName;
        if (twitterCard != null) TwitterCard = twitterCard;
        if (twitterSite != null) TwitterSite = twitterSite;
        if (twitterCreator != null) TwitterCreator = twitterCreator;
        if (ogLocale != null) OgLocale = ogLocale;
        if (charset != null) Charset = charset;
    }
}

/// <summary>
/// Represents an Open Graph Image
/// </summary>
public sealed class OgImage
{
    public string Url { get; }
    public string? Type { get; }
    public string Width { get; }
    public string Height { get; }

    public OgImage(string url, string width, string height, string? type = null)
    {
        Url = url;
        Width = width;
        Height = height;
        if (type != null) Type = type;
    }
}
