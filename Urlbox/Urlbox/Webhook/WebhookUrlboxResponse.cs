using System.Text.Json.Serialization;

namespace Screenshots;

public sealed class WebhookUrlboxResponse
{
    public string Event { get; }
    public string RenderId { get; }
    public WebhookError Error { get; }
    public SyncUrlboxResponse Result { get; }
    public Meta Meta { get; }

    [JsonConstructor]
    public WebhookUrlboxResponse(
        string Event,
        string renderId,
        Meta meta,
        SyncUrlboxResponse result = null,
        WebhookError error = null
    )
    {
        if (result != null && error != null)
        {
            throw new ArgumentException("The WebhookUrlboxResponse must have one of Error or Response, not both.");
        }

        this.Event = Event;
        RenderId = renderId;
        Meta = meta;
        Result = result;
        Error = error;
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