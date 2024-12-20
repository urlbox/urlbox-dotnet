using System.Text.Json.Serialization;

namespace UrlboxSDK.Metadata.Resource;

/// <summary>
/// Represents Metadata for a Urlbox Response when save_metadata or metadata options are set to true
/// </summary>
public sealed class UrlboxMetadata
{
    public string UrlRequested { get; }
    public string UrlResolved { get; }
    public string Url { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Author { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Date { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Image { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Logo { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Publisher { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Title { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OgTitle { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OgImage[]? OgImage { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OgDescription { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OgUrl { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OgType { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OgSiteName { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OgLocale { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Charset { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TwitterCard { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TwitterSite { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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

