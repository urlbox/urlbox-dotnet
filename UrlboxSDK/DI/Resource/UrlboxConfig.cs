using UrlboxSDK.Resource;

namespace UrlboxSDK.DI.Resource;

/// <summary>
/// Represents the config settings for Urlbox, specifically for DI.
/// Encapsulates config details, making them easy to manage and inject into services 
/// instead of passing parameters directly to the <see cref="Urlbox"/> constructor.
/// </summary>
public class UrlboxConfig
{
    public string? Key { get; set; }
    public string? Secret { get; set; }
    public string? WebhookSecret { get; set; }
    public string BaseUrl { get; set; } = Urlbox.BASE_URL;

    /// <summary>
    /// Allows for parameterless construction of UrlboxConfig while still validating presence of key/secret
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
        {
            throw new ArgumentException("UrlboxConfig.Key is required and cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(Secret))
        {
            throw new ArgumentException("UrlboxConfig.Secret is required and cannot be null or empty.");
        }
    }
}