using System.Text.Json.Serialization;
using UrlboxSDK.Metadata.Resource;

namespace UrlboxSDK.Response.Resource;
/// <summary>
/// abstract class for Urlbox response types.
/// </summary>
public abstract class AbstractUrlboxResponse
{
    protected const string EXTENSION_HTML = ".html";
    protected const string EXTENSION_MHTML = ".mhtml";
    protected const string EXTENSION_MARKDOWN = ".md";
    protected const string EXTENSION_METADATA = ".json";

    /// <summary>
    /// Checks that a given URL has its relevant file extension
    /// </summary>
    /// <param name="url">URL to check</param>
    /// <param name="extension">Expected file extension</param>
    /// <returns>Validated URL</returns>
    /// <exception cref="ArgumentException">Thrown if URL does not contain expected extension</exception>
    protected string CheckExtension(string url, string extension)
    {
        if (!url.Contains(extension))
        {
            throw new ArgumentException($"The URL {url} does not contain the extension {extension}");
        }
        return url;
    }

    public string? HtmlUrl { get; }
    public string? MhtmlUrl { get; }
    public string? MetadataUrl { get; }
    public string? MarkdownUrl { get; }
    public UrlboxMetadata? Metadata { get; }

    [JsonConstructor]
    protected AbstractUrlboxResponse(
        string? htmlUrl = null,
        string? mhtmlUrl = null,
        string? metadataUrl = null,
        string? markdownUrl = null,
        UrlboxMetadata? metadata = null
    )
    {
        HtmlUrl = string.IsNullOrEmpty(htmlUrl) ? null : CheckExtension(htmlUrl, EXTENSION_HTML);
        MhtmlUrl = string.IsNullOrEmpty(mhtmlUrl) ? null : CheckExtension(mhtmlUrl, EXTENSION_MHTML);
        MetadataUrl = string.IsNullOrEmpty(metadataUrl) ? null : CheckExtension(metadataUrl, EXTENSION_METADATA);
        MarkdownUrl = string.IsNullOrEmpty(markdownUrl) ? null : CheckExtension(markdownUrl, EXTENSION_MARKDOWN);
        Metadata = metadata;
    }
}

