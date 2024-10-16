using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Screenshots
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Urlbox"/> class with the provided API key and secret.
    /// </summary>
    /// <param name="key">Your Urlbox.com API Key.</param>
    /// <param name="secret">Your Urlbox.com API Secret.</param>
    /// <param name="webhookSecret">Your Urlbox.com webhook Secret.</param>
    /// <exception cref="ArgumentException">Thrown when the API key or secret is invalid.</exception>
    public class Urlbox
    {
        private String key;
        private String secret;
        private String webhookSecret;
        private UrlGenerator urlGenerator;

        private HttpClient httpClient;

        private const string BASE_URL = "https://api.urlbox.com";
        private const string SYNC_ENDPOINT = "/v1/render/sync";
        private const string ASYNC_ENDPOINT = "/v1/render/async";

        public Urlbox(string key, string secret, string webhookSecret)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Please provide your Urlbox.com API Key");
            }
            if (String.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Please provide your Urlbox.com API Secret");
            }
            this.key = key;
            this.secret = secret;
            this.webhookSecret = webhookSecret;
            this.urlGenerator = new UrlGenerator(key, secret);
            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// Downloads a screenshot as a Base64-encoded string from a Urlbox render link.
        /// </summary>
        /// <param name="options">The options for the screenshot</param>
        /// <param name="format">The image format (e.g., "png", "jpg").</param>
        /// <returns>A Base64-encoded string of the screenshot.</returns>
        public async Task<string> DownloadAsBase64(UrlboxOptions options, string format = "png")
        {
            var urlboxUrl = this.GenerateUrlboxUrl(options, format);
            return await DownloadAsBase64(urlboxUrl);
        }

        /// <summary>
        /// Downloads a screenshot as a Base64-encoded string from the given Urlbox URL.
        /// </summary>
        /// <param name="urlboxUrl">The render link Urlbox URL.</param>
        /// <returns>A Base64-encoded string of the screenshot.</returns>
        public async Task<string> DownloadAsBase64(string urlboxUrl)
        {
            Func<HttpResponseMessage, Task<string>> onSuccess = async (result) =>
            {
                var bytes = await result.Content.ReadAsByteArrayAsync();
                var contentType = result.Content.Headers.ToDictionary(l => l.Key, k => k.Value)["Content-Type"];
                var base64 = contentType.First() + ";base64," + Convert.ToBase64String(bytes);
                return base64;
            };
            return await this.Download(urlboxUrl, onSuccess);
        }

        /// <summary>
        /// Downloads a screenshot and saves it as a file.
        /// </summary>
        /// <param name="options">The options for the screenshot.</param>
        /// <param name="filename">The file path where the screenshot will be saved.</param>
        /// <param name="format">The image format (e.g., "png", "jpg"). Default is "png".</param>
        /// <returns>The contents of the downloaded file as a string.</returns>
        public async Task<string> DownloadToFile(UrlboxOptions options, string filename, string format = "png")
        {
            var urlboxUrl = GenerateUrlboxUrl(options, format);
            return await DownloadToFile(urlboxUrl, filename);
        }

        /// <summary>
        /// Downloads a screenshot from the given Urlbox URL and saves it as a file.
        /// </summary>
        /// <param name="urlboxUrl">The render link Urlbox URL.</param>
        /// <param name="filename">The file path where the screenshot will be saved.</param>
        /// <returns>The contents of the downloaded file.</returns>
        public async Task<string> DownloadToFile(string urlboxUrl, string filename)
        {
            Func<HttpResponseMessage, Task<string>> onSuccess = async (result) =>
            {
                using (
                        Stream contentStream = await result.Content.ReadAsStreamAsync(),
                        stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(stream);
                }
                return await result.Content.ReadAsStringAsync();
            };
            return await Download(urlboxUrl, onSuccess);
        }

        /// <summary>
        /// Downloads content from the given Urlbox render link and processes it using the provided onSuccess function.
        /// </summary>
        /// <param name="urlboxUrl">The render link Urlbox URL.</param>
        /// <param name="onSuccess">The function to execute when the download is successful.</param>
        /// <returns>The result of the success function.</returns>
        private async Task<string> Download(string urlboxUrl, Func<HttpResponseMessage, Task<string>> onSuccess)
        {
            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(urlboxUrl).ConfigureAwait(false))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        Debug.WriteLine(result, "SUCCESS!");
                        return await onSuccess(result);
                    }
                    else
                    {
                        Debug.WriteLine(result, "FAIL");
                        return "FAIL";
                    }
                }
            }
        }

        /// <summary>
        /// Generates a URL for a PNG screenshot using the provided options.
        /// </summary>
        /// <param name="options">The options for the screenshot.</param>
        /// <returns>A render link Url to render a PNG screenshot.</returns>
        public string GeneratePNGUrl(UrlboxOptions options)
        {
            return GenerateUrlboxUrl(options, "png");
        }

        /// <summary>
        /// Generates a URL for a JPEG screenshot using the provided options.
        /// </summary>
        /// <param name="options">The options for the screenshot.</param>
        /// <returns>A render link Url to render a JPEG screenshot.</returns>
        public string GenerateJPEGUrl(UrlboxOptions options)
        {
            return GenerateUrlboxUrl(options, "jpg");
        }

        /// <summary>
        /// Generates a URL for a PDF file using the provided options.
        /// </summary>
        /// <param name="options">The options for generating the PDF.</param>
        /// <returns>A render link Url to render a PDF file.</returns>
        public string GeneratePDFUrl(UrlboxOptions options)
        {
            return GenerateUrlboxUrl(options, "pdf");
        }

        /// <summary>
        /// Generates a Urlbox URL with the specified format.
        /// </summary>
        /// <param name="options">The options for generating the screenshot or PDF.</param>
        /// <param name="format">The format of the output, e.g., "png", "jpg", "pdf".</param>
        /// <returns>A render link URL to render the content.</returns>
        public string GenerateUrlboxUrl(UrlboxOptions options, string format = "png")
        {
            return urlGenerator.GenerateUrlboxUrl(options, format);
        }

        /// <summary>
        /// Sends a synchronous render request to the Urlbox API and returns the rendered screenshot url and size.
        /// </summary>
        /// <param name="options">An instance of <see cref="UrlboxOptions"/> that contains the options for the render request.</param>
        /// <returns>A <see cref="SyncUrlboxResponse"/> containing the result of the render request.</returns>
        /// <exception cref="Exception">Thrown when the response is of an asynchronous type, indicating an incorrect endpoint was called.</exception>
        /// <remarks>
        /// This method makes an HTTP POST request to the v1/render/sync endpoint, expecting a synchronous response. 
        /// </remarks>
        public async Task<SyncUrlboxResponse> Render(UrlboxOptions options)
        {
            IUrlboxResponse result = await this.MakeUrlboxPostRequest(SYNC_ENDPOINT, options);
            if (result is SyncUrlboxResponse syncResponse)
            {
                return syncResponse;
            }
            throw new Exception("Rendered /async when should've rendered /sync.");
        }

        /// <summary>
        /// Sends an asynchronous render request to the Urlbox API and returns the status of the render request, as 
        /// well as a renderId and a statusUrl which can be polled to find out when the render succeeds.
        /// </summary>
        /// <param name="options">An instance of <see cref="UrlboxOptions"/> that contains the options for the render request.</param>
        /// <returns>A <see cref="AsyncUrlboxResponse"/> containing the result of the asynchronous render request, including the statusUrl, status and renderId.</returns>
        /// <exception cref="Exception">Thrown when the response is of a synchronous type, indicating an incorrect endpoint was called.</exception>
        /// <remarks>
        /// This method makes an HTTP POST request to the /render/async endpoint, expecting an asynchronous response. 
        /// </remarks>
        public async Task<AsyncUrlboxResponse> RenderAsync(UrlboxOptions options)
        {
            IUrlboxResponse result = await this.MakeUrlboxPostRequest(ASYNC_ENDPOINT, options);
            if (result is AsyncUrlboxResponse asyncResponse)
            {
                return asyncResponse;
            }
            throw new Exception("Rendered /sync when should've rendered /async.");
        }

        /// <summary>
        /// Makes an HTTP POST request to the Urlbox API endpoint and returns the response as a <see cref="UrlboxResponse"/> object.
        /// </summary>
        /// <param name="endpoint">The Urlbox API endpoint to send the request to. Must be either /render/sync or /render/async.</param>
        /// <param name="options">The <see cref="UrlboxOptions"/> object containing the configuration options for the API request.</param>
        /// <returns>A <see cref="IUrlboxResponse"/> object containing the result of the API call, which includes the rendered URL and additional data.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid endpoint is provided or when the request fails with a non-successful response code.</exception>
        /// <remarks>
        /// The method first validates the endpoint, then constructs the request with the provided options, serializing them to JSON using the snake_case naming policy. 
        /// The request is authenticated via a Bearer token, and the response is deserialized from camelCase to PascalCase to fit C# conventions.
        /// </remarks>
        private async Task<IUrlboxResponse> MakeUrlboxPostRequest(string endpoint, UrlboxOptions options)
        {
            if (endpoint != SYNC_ENDPOINT && endpoint != ASYNC_ENDPOINT)
            {
                throw new ArgumentException("Endpoint must be one of /render/sync or /render/async.");
            }
            string url = BASE_URL + endpoint;

            JsonSerializerOptions serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                WriteIndented = true
            };

            string optionsAsJson = JsonSerializer.Serialize(options, serializeOptions);

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new StringContent(optionsAsJson, Encoding.UTF8, "application/json");

                request.Headers.Add("Authorization", $"Bearer {this.secret}");

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    var deserializerOptions = new JsonSerializerOptions
                    {
                        // Convert camelCase JSON response to PascalCase class convention
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };
                    return endpoint switch
                    {
                        SYNC_ENDPOINT => JsonSerializer.Deserialize<SyncUrlboxResponse>(responseData, deserializerOptions),
                        ASYNC_ENDPOINT => JsonSerializer.Deserialize<AsyncUrlboxResponse>(responseData, deserializerOptions),
                        _ => throw new ArgumentException("Invalid endpoint."),
                    };
                }
                else
                {
                    throw new ArgumentException($"Could not make post request to {url}: {response}");
                }
            }
        }

    }

    /// <summary>
    /// Interface for Urlbox response types.
    /// Allows one response type for makeUrlboxPostRequest which can then
    /// be cast to the specific /sync or /async response
    /// Implementations represent either synchronous or asynchronous responses.
    /// </summary>
    public interface IUrlboxResponse
    {
    }

    /// <summary>
    /// Represents a synchronous Urlbox response.
    /// </summary>
    public class SyncUrlboxResponse : IUrlboxResponse
    {
        public string RenderUrl { get; set; }
        public int Size { get; set; }
    }

    /// <summary>
    /// Represents an asynchronous Urlbox response.
    /// </summary>
    public class AsyncUrlboxResponse : IUrlboxResponse
    {
        public string Status { get; set; }
        public string RenderId { get; set; }
        public string StatusUrl { get; set; }
    }


    /// <summary>
    /// Initializes a new instance of the UrlboxOptions. These are used as part of any Urlbox method which requires render options.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the Url OR Html option isn't passed in on init.</exception>
    public class UrlboxOptions
    {

        public UrlboxOptions(string url = null, string html = null)
        {
            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(html))
            {
                throw new ArgumentException("Either of options 'url' or 'html' must be provided.");
            }
            Url = url;
            Html = html;
        }

        public string Url { get; set; }
        public string Html { get; set; }
        public string Format { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool FullPage { get; set; }
        public string Selector { get; set; }
        public string Clip { get; set; }
        public bool Gpu { get; set; }
        public string ResponseType { get; set; }
        public bool BlockAds { get; set; }
        public bool HideCookieBanners { get; set; }
        public bool ClickAccept { get; set; }
        public bool BlockUrls { get; set; }
        public bool BlockImages { get; set; }
        public bool BlockFonts { get; set; }
        public bool BlockMedias { get; set; }
        public bool BlockStyles { get; set; }
        public bool BlockScripts { get; set; }
        public bool BlockFrames { get; set; }
        public bool BlockFetch { get; set; }
        public bool BlockXhr { get; set; }
        public bool BlockSockets { get; set; }
        public string HideSelector { get; set; }
        public string Js { get; set; }
        public string Css { get; set; }
        public bool DarkMode { get; set; }
        public bool ReducedMotion { get; set; }
        public bool Retina { get; set; }
        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }
        public string ImgFit { get; set; }
        public string ImgPosition { get; set; }
        public string ImgBg { get; set; }
        public int ImgPad { get; set; }
        public int Quality { get; set; }
        public bool Transparent { get; set; }
        public int MaxHeight { get; set; }
        public string Download { get; set; }
        public string PdfPageSize { get; set; }
        public string PdfPageRange { get; set; }
        public int PdfPageWidth { get; set; }
        public int PdfPageHeight { get; set; }
        public string PdfMargin { get; set; }
        public int PdfMarginTop { get; set; }
        public int PdfMarginRight { get; set; }
        public int PdfMarginBottom { get; set; }
        public int PdfMarginLeft { get; set; }
        public bool PdfAutoCrop { get; set; }
        public double PdfScale { get; set; }
        public string PdfOrientation { get; set; }
        public bool PdfBackground { get; set; }
        public bool DisableLigatures { get; set; }
        public string Media { get; set; }
        public bool PdfShowHeader { get; set; }
        public string PdfHeader { get; set; }
        public bool PdfShowFooter { get; set; }
        public string PdfFooter { get; set; }
        public bool Readable { get; set; }
        public bool Force { get; set; }
        public string Unique { get; set; }
        public int Ttl { get; set; }
        public string Proxy { get; set; }
        public string Header { get; set; }
        public string Cookie { get; set; }
        public string UserAgent { get; set; }
        public string Platform { get; set; }
        public string AcceptLang { get; set; }
        public string Authorization { get; set; }
        public string Tz { get; set; }
        public string EngineVersion { get; set; }
        public int Delay { get; set; }
        public int Timeout { get; set; }
        public string WaitUntil { get; set; }
        public string WaitFor { get; set; }
        public string WaitToLeave { get; set; }
        public int WaitTimeout { get; set; }
        public bool FailIfSelectorMissing { get; set; }
        public bool FailIfSelectorPresent { get; set; }
        public bool FailOn4xx { get; set; }
        public bool FailOn5xx { get; set; }
        public string ScrollTo { get; set; }
        public string Click { get; set; }
        public string ClickAll { get; set; }
        public string Hover { get; set; }
        public string BgColor { get; set; }
        public bool DisableJs { get; set; }
        public string FullPageMode { get; set; }
        public bool FullWidth { get; set; }
        public bool AllowInfinite { get; set; }
        public bool SkipScroll { get; set; }
        public bool DetectFullHeight { get; set; }
        public int MaxSectionHeight { get; set; }
        public string ScrollIncrement { get; set; }
        public int ScrollDelay { get; set; }
        public string Highlight { get; set; }
        public string HighlightFg { get; set; }
        public string HighlightBg { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Accuracy { get; set; }
        public bool UseS3 { get; set; }
        public string S3Path { get; set; }
        public string S3Bucket { get; set; }
        public string S3Endpoint { get; set; }
        public string S3Region { get; set; }
        public string CdnHost { get; set; }
        public string S3StorageClass { get; set; }
    }


    /// <summary>
    /// A custom naming policy for converting property names from PascalCase to snake_case
    /// when serializing JSON.
    /// 
    /// <remarks>
    /// This JsonNamingPolicy is included by default in .NET 8.0 (JsonNamingPolicy.SnakeCaseLower).
    /// However, a custom implementation has been made here to maintain compatibility with .NET 6.0,
    /// which is still under Long-Term Support (LTS). Keeping the SDK at 6.0 ensures broader accessibility 
    /// for audiences still using this version.
    /// </remarks>
    /// </summary>
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            // Convert PascalCase to snake_case
            return string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }
}