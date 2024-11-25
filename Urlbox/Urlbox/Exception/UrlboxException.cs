using System.Text.Json.Serialization;

namespace UrlboxSDK;

public sealed class UrlboxException : Exception
{
    public string RequestId { get; }
    public string Code { get; }
    public string Errors { get; }

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
        if (root?.Error == null || string.IsNullOrWhiteSpace(root?.Error.Message) || string.IsNullOrWhiteSpace(root.RequestId))
            throw new JsonException("Invalid JSON response structure");

        throw new UrlboxException(root.Error, root.RequestId);
    }

    public sealed class UrlboxError
    {
        [JsonPropertyName("message")]
        public string Message { get; init; }

        [JsonPropertyName("code")]
        public string Code { get; init; }

        [JsonPropertyName("errors")]
        public string Errors { get; init; }
    }

    private sealed class RawResponse
    {
        [JsonPropertyName("error")]
        public UrlboxError Error { get; init; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; init; }
    }
}
