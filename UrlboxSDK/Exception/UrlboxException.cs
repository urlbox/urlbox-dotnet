using System.Text.Json.Serialization;

namespace UrlboxSDK.Exception;

public sealed class UrlboxException : System.Exception
{
    public string RequestId { get; }
    public string? Code { get; }
    public string? Errors { get; }

    public UrlboxException(UrlboxError error, string requestId)
        : base(error.Message)
    {
        RequestId = requestId ?? "Unknown Request ID";
        if (!string.IsNullOrEmpty(error.Code)) Code = error.Code;
        if (!string.IsNullOrEmpty(error.Errors)) Errors = error.Errors;
    }

    public static UrlboxException FromResponse(string response, JsonSerializerOptions deserializerOptions)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new ArgumentException("Response cannot be null or empty", nameof(response));

        var root = JsonSerializer.Deserialize<RawResponse>(response, deserializerOptions);
        if (root == null || root?.Error == null || string.IsNullOrWhiteSpace(root?.Error.Message) || string.IsNullOrWhiteSpace(root.RequestId))
        {
            throw new JsonException("Invalid JSON response structure");
        }

        throw new UrlboxException(root.Error, root.RequestId);
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

    private sealed class RawResponse
    {
        [JsonPropertyName("error")]
        public UrlboxError Error { get; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; }

        [JsonConstructor]
        public RawResponse(UrlboxError error, string requestId)
        {
            Error = error;
            RequestId = requestId;
        }
    }
}
