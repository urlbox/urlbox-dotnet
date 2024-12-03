using System.Text.Json.Serialization;

namespace UrlboxSDK.Webhook.Resource;

/// <summary>
/// Represents the Metadata that comes back from Urlbox's Webhook Response
/// </summary>
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
