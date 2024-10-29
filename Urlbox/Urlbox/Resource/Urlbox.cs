using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        private UrlboxWebhookValidator urlboxWebhookValidator;

        private HttpClient httpClient;

        private const string BASE_URL = "https://api.urlbox.com";
        private const string SYNC_ENDPOINT = "/v1/render/sync";
        private const string ASYNC_ENDPOINT = "/v1/render/async";

        public Urlbox(string key, string secret, string webhookSecret = null)
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
            this.httpClient = new HttpClient();
            if (!String.IsNullOrEmpty(webhookSecret))
            {
                this.webhookSecret = webhookSecret;
                this.urlboxWebhookValidator = new UrlboxWebhookValidator(webhookSecret);
            }
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
                using (HttpResponseMessage result = await client.GetAsync(urlboxUrl).ConfigureAwait(false))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        return await onSuccess(result);
                    }
                    else
                    {
                        IEnumerable<string> values;
                        var errorMessage = result.Headers.TryGetValues("x-urlbox-error-message", out values);
                        throw new Exception($"Request failed: {values.FirstOrDefault()}");
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

        /// <summary>
        /// A static method to create a new instance of the Urlbox class
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiSecret"></param>
        /// <param name="webhookSecret"></param>
        /// <param name="client"></param>
        /// <returns>A new instance of the Urlbox class.</returns>
        /// <exception cref="ArgumentException">Thrown when there is no api key or secret</exception>
        public static Urlbox FromCredentials(string apiKey, string apiSecret, string webhookSecret)
        {
            return new Urlbox(apiKey, apiSecret, webhookSecret);
        }

        /// <summary>
        /// Verifies a webhook responses' x-urlbox-signature header to ensure it came from Urlbox
        /// </summary>
        /// <param name="header"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool verifyWebhookSignature(string header, string content)
        {
            if (!(this.urlboxWebhookValidator is UrlboxWebhookValidator))
            {
                throw new ArgumentException("Please set your webhook secret in the Urlbox instance before calling this method.");
            }
            return this.urlboxWebhookValidator.verifyWebhookSignature(header, content);
        }
    }
}