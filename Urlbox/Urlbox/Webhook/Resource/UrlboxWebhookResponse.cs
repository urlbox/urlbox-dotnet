using System.Text.Json.Serialization;

namespace UrlboxSDK;

public sealed class UrlboxWebhookResponse
{
    public string Event { get; }
    public string RenderId { get; }
    public WebhookError? Error { get; }
    public SyncUrlboxResponse? Result { get; }
    public Meta Meta { get; }

    [JsonConstructor]
    public UrlboxWebhookResponse(
        string Event,
        string renderId,
        Meta meta,
        SyncUrlboxResponse? result = null,
        WebhookError? error = null
    )
    {
        if (result != null && error != null)
        {
            throw new ArgumentException("The UrlboxWebhookResponse must have one of Error or Response, not both.");
        }

        this.Event = Event;
        RenderId = renderId;
        Meta = meta;
        if (result != null) Result = result;
        if (error != null) Error = error;
    }
}

public sealed class WebhookError
{
    public string Message { get; }

    [JsonConstructor]
    public WebhookError(string message)
    {
        Message = message;
    }
}

public sealed class Meta
{
    public string StartTime { get; }
    public string EndTime { get; }

    [JsonConstructor]
    public Meta(string startTime, string endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}
