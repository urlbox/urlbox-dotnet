using System.Collections.Generic;
using System.Threading.Tasks;
using UrlboxSDK.Metadata.Resource;
using UrlboxSDK.Options.Resource;
using UrlboxSDK.Response.Resource;
using UrlboxSDK.Webhook.Resource;

namespace UrlboxSDK;

public interface IUrlbox
{
    // Screenshot and File Generation Methods
    Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options);
    Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options, int timeout);
    Task<AsyncUrlboxResponse> TakePdf(UrlboxOptions options);
    Task<AsyncUrlboxResponse> TakeMp4(UrlboxOptions options);
    Task<SyncUrlboxResponse> Render(UrlboxOptions options);
    Task<SyncUrlboxResponse> Render(IDictionary<string, object> options);
    Task<AsyncUrlboxResponse> RenderAsync(UrlboxOptions options);
    Task<AsyncUrlboxResponse> RenderAsync(IDictionary<string, object> options);
    Task<AsyncUrlboxResponse> TakeScreenshotWithMetadata(UrlboxOptions options);

    // Extraction Methods

    Task<UrlboxMetadata> ExtractMetadata(UrlboxOptions options);
    Task<string> ExtractMarkdown(UrlboxOptions options);
    Task<string> ExtractHtml(UrlboxOptions options);
    Task<string> ExtractMhtml(UrlboxOptions options);

    // Download and File Handling Methods
    Task<string> DownloadAsBase64(UrlboxOptions options, string format = "png", bool sign = true);
    Task<string> DownloadAsBase64(string urlboxUrl);
    Task<string> DownloadToFile(string urlboxUrl, string filename);
    Task<string> DownloadToFile(UrlboxOptions options, string filename, string format = "png", bool sign = true);

    // URL Generation Methods
    string GeneratePNGUrl(UrlboxOptions options, bool sign = true);
    string GenerateJPEGUrl(UrlboxOptions options, bool sign = true);
    string GeneratePDFUrl(UrlboxOptions options, bool sign = true);
    string GenerateRenderLink(UrlboxOptions options, string format = "png", bool sign = true);

    // Status and Validation Methods
    Task<AsyncUrlboxResponse> GetStatus(string statusUrl);
    UrlboxWebhookResponse VerifyWebhookSignature(string header, string content);
}
