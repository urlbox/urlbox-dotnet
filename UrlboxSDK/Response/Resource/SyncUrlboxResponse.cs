using System.Text.Json.Serialization;
using UrlboxSDK.Metadata.Resource;

namespace UrlboxSDK.Response.Resource;

/// <summary>
/// Represents a synchronous Urlbox response.
/// </summary>
public sealed class SyncUrlboxResponse : AbstractUrlboxResponse
{
    /// <summary>
    /// The location of the screenshot
    /// </summary>
    public string RenderUrl { get; }
    /// <summary>
    /// The size of the screenshot in bytes
    /// </summary>
    public int Size { get; }

    [JsonConstructor]
    public SyncUrlboxResponse(
        string renderUrl,
        int size,
        string? htmlUrl = null,
        string? mhtmlUrl = null,
        string? metadataUrl = null,
        string? markdownUrl = null,
        UrlboxMetadata? metadata = null
    ) : base(htmlUrl, mhtmlUrl, metadataUrl, markdownUrl, metadata)
    {
        RenderUrl = renderUrl;
        Size = size;
    }
}