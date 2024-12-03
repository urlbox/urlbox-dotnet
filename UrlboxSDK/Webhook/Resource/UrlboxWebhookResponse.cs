using System.Text.Json.Serialization;
using UrlboxSDK.Response.Resource;

namespace UrlboxSDK.Webhook.Resource;

public sealed class UrlboxWebhookResponse
{
    public string Event { get; }
    public string RenderId { get; }
    public ErrorUrlboxResponse.UrlboxError? Error { get; }
    public SyncUrlboxResponse? Result { get; }
    public Meta Meta { get; }

    [JsonConstructor]
    public UrlboxWebhookResponse(
        string @event,
        string renderId,
        Meta meta,
        SyncUrlboxResponse? result = null,
        ErrorUrlboxResponse.UrlboxError? error = null
    )
    {
        if (result != null && error != null)
        {
            throw new ArgumentException("The UrlboxWebhookResponse must have one of Error or Response, not both.");
        }

        Event = @event;
        RenderId = renderId;
        Meta = meta;
        if (result != null) Result = result;
        if (error != null) Error = error;
    }
}
