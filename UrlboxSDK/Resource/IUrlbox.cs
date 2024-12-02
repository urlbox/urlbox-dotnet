using System.Threading.Tasks;
using UrlboxSDK.Options.Resource;
using UrlboxSDK.Response.Resource;
using UrlboxSDK.Webhook.Resource;

namespace UrlboxSDK.Resource;

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
    Task<string> DownloadAsBase64(UrlboxOptions options, string format = "png", bool sign = false);
    Task<string> DownloadAsBase64(string urlboxUrl);
    Task<string> DownloadToFile(string urlboxUrl, string filename);
    Task<string> DownloadToFile(UrlboxOptions options, string filename, string format = "png", bool sign = false);

    // URL Generation Methods
    string GeneratePNGUrl(UrlboxOptions options, bool sign = false);
    string GenerateJPEGUrl(UrlboxOptions options, bool sign = false);
    string GeneratePDFUrl(UrlboxOptions options, bool sign = false);
    string GenerateRenderLink(UrlboxOptions options, string format = "png", bool sign = false);

    // Status and Validation Methods
    Task<AsyncUrlboxResponse> GetStatus(string statusUrl);
    UrlboxWebhookResponse VerifyWebhookSignature(string header, string content);
}
