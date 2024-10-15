using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Screenshots
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Urlbox"/> class with the provided API key and secret.
    /// </summary>
    /// <param name="key">Your Urlbox.com API Key.</param>
    /// <param name="secret">Your Urlbox.com API Secret.</param>
    /// <exception cref="ArgumentException">Thrown when the API key or secret is invalid.</exception>
    public class Urlbox
    {
        private String key;
        private String secret;
        private UrlGenerator urlGenerator;

        public Urlbox(string key, string secret)
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
            this.urlGenerator = new UrlGenerator(key, secret);
        }

        /// <summary>
        /// Downloads a screenshot as a Base64-encoded string from a Urlbox render link.
        /// </summary>
        /// <param name="options">The options for the screenshot</param>
        /// <param name="format">The image format (e.g., "png", "jpg").</param>
        /// <returns>A Base64-encoded string of the screenshot.</returns>
        public async Task<string> DownloadAsBase64(IDictionary<string, object> options, string format = "png")
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
        public async Task<string> DownloadToFile(IDictionary<string, object> options, string filename, string format = "png")
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
        public string GeneratePNGUrl(IDictionary<string, object> options)
        {
            return GenerateUrlboxUrl(options, "png");
        }

        /// <summary>
        /// Generates a URL for a JPEG screenshot using the provided options.
        /// </summary>
        /// <param name="options">The options for the screenshot.</param>
        /// <returns>A render link Url to render a JPEG screenshot.</returns>
        public string GenerateJPEGUrl(IDictionary<string, object> options)
        {
            return GenerateUrlboxUrl(options, "jpg");
        }

        /// <summary>
        /// Generates a URL for a PDF file using the provided options.
        /// </summary>
        /// <param name="options">The options for generating the PDF.</param>
        /// <returns>A render link Url to render a PDF file.</returns>
        public string GeneratePDFUrl(IDictionary<string, object> options)
        {
            return GenerateUrlboxUrl(options, "pdf");
        }

        /// <summary>
        /// Generates a Urlbox URL with the specified format.
        /// </summary>
        /// <param name="options">The options for generating the screenshot or PDF.</param>
        /// <param name="format">The format of the output, e.g., "png", "jpg", "pdf".</param>
        /// <returns>A render link URL to render the content.</returns>
        public string GenerateUrlboxUrl(IDictionary<string, object> options, string format = "png")
        {
            return urlGenerator.GenerateUrlboxUrl(options, format);
        }
    }
}