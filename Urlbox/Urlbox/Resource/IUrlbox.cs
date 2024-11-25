using System.Threading.Tasks;

namespace UrlboxSDK;

public interface IUrlbox
{
    // Screenshot and File Generation Methods
    Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options);
    Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options, int timeout);
    Task<AsyncUrlboxResponse> TakePdf(UrlboxOptions options);
    Task<AsyncUrlboxResponse> TakeMp4(UrlboxOptions options);
    Task<AsyncUrlboxResponse> TakeFullPageScreenshot(UrlboxOptions options);
    Task<AsyncUrlboxResponse> TakeMobileScreenshot(UrlboxOptions options);
    Task<AsyncUrlboxResponse> TakeScreenshotWithMetadata(UrlboxOptions options);
    Task<SyncUrlboxResponse> Render(UrlboxOptions options);
    Task<AsyncUrlboxResponse> RenderAsync(UrlboxOptions options);

    // Download and File Handling Methods
    Task<string> DownloadAsBase64(UrlboxOptions options, string format = "png");
    Task<string> DownloadAsBase64(string urlboxUrl);
    Task<string> DownloadToFile(string urlboxUrl, string filename);
    Task<string> DownloadToFile(UrlboxOptions options, string filename, string format = "png");

    // URL Generation Methods
    string GeneratePNGUrl(UrlboxOptions options);
    string GenerateJPEGUrl(UrlboxOptions options);
    string GeneratePDFUrl(UrlboxOptions options);
    string GenerateUrlboxUrl(UrlboxOptions options, string format = "png");

    // Status and Validation Methods
    Task<AsyncUrlboxResponse> GetStatus(string statusUrl);
    WebhookUrlboxResponse VerifyWebhookSignature(string header, string content);
}
