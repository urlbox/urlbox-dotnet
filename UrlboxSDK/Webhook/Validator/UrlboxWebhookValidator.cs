using System.Security.Cryptography;

namespace UrlboxSDK.Webhook.Validator;
/// <summary>
/// A class encompassing webhook validation logic.
/// </summary>
public sealed class UrlboxWebhookValidator
{
    private string webhookSecret;

    /// <summary>
    /// Constructs a UrlboxWebhookValidator
    /// </summary>
    /// <param name="secret"></param>
    /// <exception cref="ArgumentException"></exception>
    public UrlboxWebhookValidator(string secret)
    {
        if (String.IsNullOrEmpty(secret))
        {
            throw new ArgumentException("Unable to verify signature as Webhook Secret is not set. You can find your webhook secret inside your project\'s settings - https://www.urlbox.com/dashboard/projects");
        }
        this.webhookSecret = secret;
    }

    /// <summary>
    /// Verifies the webhook signature from the request hash.
    /// </summary>
    /// <param name="header">The x-urlbox-signature header</param>
    /// <param name="content"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Thrown when there is an empty header</exception> 
    public bool VerifyWebhookSignature(string header, string content)
    {
        if (String.IsNullOrEmpty(header) || !header.Contains("t=") || !header.Contains("sha256=") || !header.Contains(","))
        {
            throw new ArgumentException("Unable to verify signature as header is empty or malformed. Please ensure you pass the `x-urlbox-signature` from the header of the webhook response.");
        }

        string timestamp = GetTimestampFromHeader(header);
        string signature = GetSignature(header);

        string generatedHash = GenerateHash(timestamp, content);
        return generatedHash == signature;
    }

    /// <summary>
    /// Method to generate the HMAC hash from the urlbox header's timestamp and content.
    /// </summary>
    /// <param name="headerTimestamp"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public string GenerateHash(string headerTimestamp, string content)
    {
        string messageToHash = headerTimestamp + "." + content;
        byte[] secretKeyBytes = Encoding.UTF8.GetBytes(this.webhookSecret);
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageToHash);

        using HMACSHA256 hmacsha256 = new(secretKeyBytes);
        byte[] hashBytes = hmacsha256.ComputeHash(messageBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();  // Convert hash to hex string
    }

    /// <summary>
    /// Method to get the signature from the x-urlbox-signature header.
    /// </summary>
    /// <param name="header"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public string GetSignature(string header)
    {
        string[] commaSplit = header.Split(',');
        string signatureWithPrefix = commaSplit[1];
        string signature = signatureWithPrefix.Split('=').Last();

        if (!signatureWithPrefix.Contains("sha256=") || String.IsNullOrEmpty(signature))
        {
            throw new ArgumentException("The signature could not be found, please ensure you are passing the x-urlbox-signature header.");
        }

        return signature;
    }

    /// <summary>
    /// Gets the timestamp from the x-urlbox-signature header.
    /// </summary>
    /// <param name="header"></param>
    /// <returns></returns>
    private string GetTimestampFromHeader(string header)
    {
        string[] commaSplit = header.Split(',');
        string timestampWithPrefix = commaSplit[0];
        string timestamp = timestampWithPrefix.Split('=').Last();

        if (!timestampWithPrefix.Contains("t=") || String.IsNullOrEmpty(timestamp))
        {
            throw new ArgumentException("The timestamp could not be found, please ensure you are passing the x-urlbox-signature header.");
        }

        return timestamp;
    }
}
