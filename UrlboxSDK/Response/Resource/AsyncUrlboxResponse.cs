using System.Text.Json.Serialization;
using UrlboxSDK.Metadata.Resource;

namespace UrlboxSDK.Response.Resource;

/// <summary>
/// Represents an asynchronous Urlbox response.
/// </summary>
public sealed class AsyncUrlboxResponse : AbstractUrlboxResponse
{
    public string Status { get; } // EG 'succeeded'
    public string RenderId { get; } // A UUID for the request
    public string StatusUrl { get; } // A url which you can poll to check the render's status
    public string? RenderUrl { get; } // only on status succeeded
    public int? Size { get; } // only on status succeeded

    [JsonConstructor]
    public AsyncUrlboxResponse(
        string status,
        string renderId,
        string statusUrl,
        int? size = null,
        string? renderUrl = null,
        string? htmlUrl = null,
        string? mhtmlUrl = null,
        string? metadataUrl = null,
        string? markdownUrl = null,
        UrlboxMetadata? metadata = null
        ) : base(htmlUrl, mhtmlUrl, metadataUrl, markdownUrl, metadata)
    {
        Status = status;
        RenderId = renderId;
        StatusUrl = statusUrl;
        Size = size;
        if (!String.IsNullOrEmpty(renderUrl)) RenderUrl = renderUrl;
    }
}
