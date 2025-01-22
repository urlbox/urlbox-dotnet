namespace UrlboxSDK.Metadata.Resource;

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
