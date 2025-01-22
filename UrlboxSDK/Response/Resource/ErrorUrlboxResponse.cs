
using System.Text.Json.Serialization;

namespace UrlboxSDK.Response.Resource;

public sealed class ErrorUrlboxResponse
{
    [JsonPropertyName("error")]
    public UrlboxError Error { get; }

    [JsonPropertyName("requestId")]
    public string RequestId { get; }

    [JsonConstructor]
    public ErrorUrlboxResponse(UrlboxError error, string requestId)
    {
        Error = error;
        RequestId = requestId;
    }

    public sealed class UrlboxError
    {
        [JsonPropertyName("message")]
        public string Message { get; }

        [JsonPropertyName("code")]
        public string? Code { get; }

        [JsonPropertyName("errors")]
        public string? Errors { get; }

        [JsonConstructor]
        public UrlboxError(string message, string? code, string? errors)
        {
            Message = message;
            Code = code;
            Errors = errors;
        }
    }
}