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
        this.UrlRequested = urlRequested ?? throw new ArgumentNullException(nameof(urlRequested));
        this.UrlResolved = urlResolved ?? throw new ArgumentNullException(nameof(urlResolved));
        this.Url = url ?? throw new ArgumentNullException(nameof(url));

        if (author != null) this.Author = author;
        if (date != null) this.Date = date;
        if (description != null) this.Description = description;
        if (image != null) this.Image = image;
        if (logo != null) this.Logo = logo;
        if (publisher != null) this.Publisher = publisher;
        if (title != null) this.Title = title;
        if (ogTitle != null) this.OgTitle = ogTitle;
        if (ogImage != null) this.OgImage = ogImage;
        if (ogDescription != null) this.OgDescription = ogDescription;
        if (ogUrl != null) this.OgUrl = ogUrl;
        if (ogType != null) this.OgType = ogType;
        if (ogSiteName != null) this.OgSiteName = ogSiteName;
        if (twitterCard != null) this.TwitterCard = twitterCard;
        if (twitterSite != null) this.TwitterSite = twitterSite;
        if (twitterCreator != null) this.TwitterCreator = twitterCreator;
        if (ogLocale != null) this.OgLocale = ogLocale;
        if (charset != null) this.Charset = charset;
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
        this.Url = url;
        this.Width = width;
        this.Height = height;
        if (type != null) this.Type = type;
    }
}
