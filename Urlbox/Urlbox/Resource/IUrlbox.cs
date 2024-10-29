using System.Threading.Tasks;

namespace Screenshots;

public interface IUrlbox
{
    // Screenshot and File Generation Methods
    Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options);
    Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options, int timeout);
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
    bool VerifyWebhookSignature(string header, string content);
}