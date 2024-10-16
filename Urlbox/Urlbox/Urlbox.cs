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
}