using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using UrlboxSDK.Exception;
using UrlboxSDK.Options.Builder;
using UrlboxSDK.Options.Resource;
using UrlboxSDK.Policy;
using UrlboxSDK.Factory;
using UrlboxSDK.Webhook.Resource;
using UrlboxSDK.Webhook.Validator;
using UrlboxSDK.Response.Resource;

namespace UrlboxSDK;
/// <summary>
/// Initializes a new instance of the <see cref="Urlbox"/> class with the provided API key and secret.
/// </summary>
/// <param name="key">Your Urlbox.com API Key.</param>
/// <param name="secret">Your Urlbox.com API Secret.</param>
/// <param name="webhookSecret">Your Urlbox.com webhook Secret.</param>
/// <exception cref="ArgumentException">Thrown when the API key or secret is invalid.</exception>
public sealed class Urlbox : IUrlbox
{
    private readonly string secret;
    private readonly RenderLinkFactory renderLinkFactory;
    private readonly UrlboxWebhookValidator? urlboxWebhookValidator;
    private readonly HttpClient httpClient;
    private readonly string baseUrl;
    public const string DOMAIN = "urlbox.com";
    public const string BASE_URL = "https://api." + DOMAIN;
    private const string SYNC_ENDPOINT = "/v1/render/sync";
    private const string ASYNC_ENDPOINT = "/v1/render/async";
    private const string STATUS_ENDPOINT = "/v1/render";
    public const int DEFAULT_TIMEOUT = 60000; // 60 seconds

    /// <summary>
    /// Static function to build the UrlboxOptions
    /// </summary>
    /// <param name="url"></param>
    /// <param name="html"></param>
    /// <returns></returns>
    public static UrlboxOptionsBuilder Options(
        string? url = null,
        string? html = null
        ) => new(url, html);

    public Urlbox(string key, string secret, string? webhookSecret = null, string? baseUrl = BASE_URL)
    {
        if (String.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Please provide your Urlbox.com API Key");
        }
        if (String.IsNullOrEmpty(secret))
        {
            throw new ArgumentException("Please provide your Urlbox.com API Secret");
        }
        this.secret = secret;
        this.baseUrl = baseUrl ?? BASE_URL;
        httpClient = new HttpClient();
        renderLinkFactory = new RenderLinkFactory(key, secret);
        if (!String.IsNullOrEmpty(webhookSecret))
        {
            urlboxWebhookValidator = new UrlboxWebhookValidator(webhookSecret);
        }
    }

    // Internal constructor (testable, allows injecting dependencies to mock http)
    internal Urlbox(string key, string secret, RenderLinkFactory renderLinkFactory, HttpClient httpClient, string? webhookSecret = null, string? baseUrl = BASE_URL)
    {
        if (String.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Please provide your Urlbox.com API Key");
        }
        if (String.IsNullOrEmpty(secret))
        {
            throw new ArgumentException("Please provide your Urlbox.com API Secret");
        }
        this.secret = secret;
        this.baseUrl = baseUrl ?? BASE_URL;
        this.renderLinkFactory = renderLinkFactory ?? throw new ArgumentNullException(nameof(renderLinkFactory));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    // STATIC

    /// <summary>
    /// A static method to create a new instance of the Urlbox class
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="apiSecret"></param>
    /// <param name="webhookSecret"></param>
    /// <param name="client"></param>
    /// <returns>A new instance of the Urlbox class.</returns>
    /// <exception cref="ArgumentException">Thrown when there is no api key or secret</exception>
    public static Urlbox FromCredentials(string apiKey, string apiSecret, string? webhookSecret, string? baseUrl = BASE_URL)
    {
        return new Urlbox(apiKey, apiSecret, webhookSecret, baseUrl);
    }

    /// <summary>
    /// Gets the x-urlbox-error-message from a request
    /// </summary>
    /// <returns>The Error message as a string</returns>
    private static string GetUrlboxErrorMessage(HttpResponseMessage response)
    {
        response.Headers.TryGetValues("x-urlbox-error-message", out IEnumerable<string>? values);

        if (values != null)
        {
            return $"Request failed: {values.FirstOrDefault()}";
        }
        return $"Request failed: No x-urlbox-error-message header found";
    }

    // PUBLIC

    // ** Screenshot and File Generation Methods **

    /// <summary>
    /// A simple method which takes a screenshot of a website.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="timeout"></param>
    /// <returns> A <see cref="AsyncUrlboxResponse"/></returns>
    /// <exception cref="TimeoutException"></exception>
    public async Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options)
    {
        return await TakeScreenshotAsyncWithTimeout(options, DEFAULT_TIMEOUT);
    }

    /// <summary>
    /// A simple method which takes a screenshot of a website.
    /// Set the timeout to stop polling Urlbox at a specified time, ensuring the screenshot was successfully captured.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="timeout"></param>
    /// <returns> A <see cref="AsyncUrlboxResponse"/></returns>
    /// <exception cref="TimeoutException"></exception>
    public async Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options, int timeout)
    {
        if (timeout > 120000 || timeout < 5000)
        {
            throw new TimeoutException("Invalid Timeout Length. Must be between 5000 (5 seconds) and 120000 (2 minutes).");
        }
        return await TakeScreenshotAsyncWithTimeout(options, timeout);
    }

    /// <summary>
    /// Private method to avoid duplication when getting screenshot async
    /// </summary>
    /// <param name="options"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="TimeoutException"></exception>
    private async Task<AsyncUrlboxResponse> TakeScreenshotAsyncWithTimeout(UrlboxOptions options, int timeout)
    {
        AsyncUrlboxResponse asyncResponse = await RenderAsync(options);
        int pollingInterval = 2000; // 2 seconds
        var startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalMilliseconds < timeout)
        {
            AsyncUrlboxResponse asyncUrlboxResponse = await GetStatus(asyncResponse.RenderId);

            if (asyncUrlboxResponse.Status == "succeeded")
            {
                return asyncUrlboxResponse;
            }

            await Task.Delay(pollingInterval);
        }
        throw new TimeoutException("The screenshot request timed out.");
    }

    /// <summary>
    /// Takes a screenshot async as a PDF
    /// </summary>
    /// <param name="options"></param>
    /// <returns > A <see cref="AsyncUrlboxResponse"></returns>
    public async Task<AsyncUrlboxResponse> TakePdf(UrlboxOptions options)
    {
        options.Format = Format.Pdf;
        return await TakeScreenshot(options);
    }

    /// <summary>
    /// Takes a screenshot async as an MP4
    /// </summary>
    /// <param name="options"></param>
    /// <returns > A <see cref="AsyncUrlboxResponse"></returns>
    public async Task<AsyncUrlboxResponse> TakeMp4(UrlboxOptions options)
    {
        options.Format = Format.Mp4;
        return await TakeScreenshot(options);
    }

    /// <summary>
    /// Takes a screenshot async with fullpage = true
    /// </summary>
    /// <param name="options"></param>
    /// <returns > A <see cref="AsyncUrlboxResponse"></returns>
    public async Task<AsyncUrlboxResponse> TakeFullPageScreenshot(UrlboxOptions options)
    {
        options.FullPage = true;
        return await TakeScreenshot(options);
    }

    /// <summary>
    /// Takes a screenshot async with width at 375 to emulate a mobile viewport
    /// </summary>
    /// <param name="options"></param>
    /// <returns > A <see cref="AsyncUrlboxResponse"></returns>
    public async Task<AsyncUrlboxResponse> TakeMobileScreenshot(UrlboxOptions options)
    {
        options.Width = 375;
        return await TakeScreenshot(options);
    }

    /// <summary>
    /// Takes a screenshot async, requesting metadata about the page
    /// </summary>
    /// <param name="options"></param>
    /// <returns > A <see cref="AsyncUrlboxResponse"></returns>
    public async Task<AsyncUrlboxResponse> TakeScreenshotWithMetadata(UrlboxOptions options)
    {
        options.Metadata = true;
        return await TakeScreenshot(options);
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
        AbstractUrlboxResponse result = await MakeUrlboxPostRequest(SYNC_ENDPOINT, options);
        return result switch
        {
            SyncUrlboxResponse syncResponse => syncResponse,
            _ => throw new System.Exception("Response expected from .Render was one of SyncUrlboxResponse."),
        };
    }

    /// <summary>
    /// Sends a synchronous render request to the Urlbox API and returns the rendered screenshot url and size.
    /// </summary>
    /// <param name="options">The configuration options for the API request.</param>
    /// <returns>A <see cref="SyncUrlboxResponse"/> containing the result of the render request.</returns>
    /// <exception cref="Exception">Thrown when the response is of an asynchronous type, indicating an incorrect endpoint was called.</exception>
    /// <remarks>
    /// This method makes an HTTP POST request to the v1/render/sync endpoint, expecting a synchronous response. 
    /// </remarks>
    public async Task<SyncUrlboxResponse> Render(IDictionary<string, object> options)
    {
        AbstractUrlboxResponse result = await MakeUrlboxPostRequest(SYNC_ENDPOINT, options);
        return result switch
        {
            SyncUrlboxResponse syncResponse => syncResponse,
            _ => throw new System.Exception("Response expected from .Render was one of SyncUrlboxResponse."),
        };
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
        AbstractUrlboxResponse result = await MakeUrlboxPostRequest(ASYNC_ENDPOINT, options);
        return result switch
        {
            AsyncUrlboxResponse asyncResponse => asyncResponse,
            _ => throw new System.Exception("Response expected from .Render was one of AsyncUrlboxResponse."),
        };
    }

    /// <summary>
    /// Sends an asynchronous render request to the Urlbox API and returns the status of the render request, as 
    /// well as a renderId and a statusUrl which can be polled to find out when the render succeeds.
    /// </summary>
    /// <param name="options">The configuration options for the API request.</param>
    /// <returns>A <see cref="AsyncUrlboxResponse"/> containing the result of the asynchronous render request, including the statusUrl, status and renderId.</returns>
    /// <exception cref="Exception">Thrown when the response is of a synchronous type, indicating an incorrect endpoint was called.</exception>
    /// <remarks>
    /// This method makes an HTTP POST request to the /render/async endpoint, expecting an asynchronous response. 
    /// </remarks>
    public async Task<AsyncUrlboxResponse> RenderAsync(IDictionary<string, object> options)
    {
        AbstractUrlboxResponse result = await MakeUrlboxPostRequest(ASYNC_ENDPOINT, options);
        return result switch
        {
            AsyncUrlboxResponse asyncResponse => asyncResponse,
            _ => throw new System.Exception("Response expected from .Render was one of AsyncUrlboxResponse."),
        };
    }

    // ** Download and File Handling Methods **

    /// <summary>
    /// Downloads a screenshot as a Base64-encoded string from a Urlbox render link.
    /// </summary>
    /// <param name="options">The options for the screenshot</param>
    /// <param name="format">The image format (e.g., "png", "jpg").</param>
    /// <returns>A Base64-encoded string of the screenshot.</returns>
    public async Task<string> DownloadAsBase64(UrlboxOptions options, string format = "png", bool sign = false)
    {
        var urlboxUrl = GenerateRenderLink(options, format, sign);
        return await DownloadAsBase64(urlboxUrl);
    }

    /// <summary>
    /// Downloads a screenshot as a Base64-encoded string from the given Urlbox URL.
    /// </summary>
    /// <param name="urlboxUrl">The render link Urlbox URL.</param>
    /// <returns>A Base64-encoded string of the screenshot.</returns>
    public async Task<string> DownloadAsBase64(string urlboxUrl)
    {
        static async Task<string> onSuccess(HttpResponseMessage result)
        {
            var bytes = await result.Content.ReadAsByteArrayAsync();
            var contentType = result.Content.Headers.ToDictionary(l => l.Key, k => k.Value)["Content-Type"];
            var base64 = contentType.First() + ";base64," + Convert.ToBase64String(bytes);
            return base64;
        }
        return await Download(urlboxUrl, onSuccess);
    }

    /// <summary>
    /// Downloads a screenshot from the given Urlbox URL and saves it as a file.
    /// </summary>
    /// <param name="urlboxUrl">The render link Urlbox URL.</param>
    /// <param name="filename">The file path where the screenshot will be saved.</param>
    /// <returns>The contents of the downloaded file.</returns>
    public async Task<string> DownloadToFile(string urlboxUrl, string filename)
    {
        async Task<string> onSuccess(HttpResponseMessage result)
        {
            using (
                    Stream contentStream = await result.Content.ReadAsStreamAsync(),
                    stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await contentStream.CopyToAsync(stream);
            }
            return await result.Content.ReadAsStringAsync();
        }
        return await Download(urlboxUrl, onSuccess);
    }

    /// <summary>
    /// Downloads a screenshot and saves it as a file.
    /// </summary>
    /// <param name="options">The options for the screenshot.</param>
    /// <param name="filename">The file path where the screenshot will be saved.</param>
    /// <param name="format">The image format (e.g., "png", "jpg"). Default is "png".</param>
    /// <returns>The contents of the downloaded file as a string.</returns>
    public async Task<string> DownloadToFile(UrlboxOptions options, string filename, string format = "png", bool sign = false)
    {
        var urlboxUrl = GenerateRenderLink(options, format, sign);
        return await DownloadToFile(urlboxUrl, filename);
    }

    // ** URL Generation Methods **

    /// <summary>
    /// Generates a URL for a PNG screenshot using the provided options.
    /// </summary>
    /// <param name="options">The options for the screenshot.</param>
    /// <returns>A render link Url to render a PNG screenshot.</returns>
    public string GeneratePNGUrl(UrlboxOptions options, bool sign = false)
    {
        return GenerateRenderLink(options, "png", sign);
    }

    /// <summary>
    /// Generates a URL for a JPEG screenshot using the provided options.
    /// </summary>
    /// <param name="options">The options for the screenshot.</param>
    /// <returns>A render link Url to render a JPEG screenshot.</returns>
    public string GenerateJPEGUrl(UrlboxOptions options, bool sign = false)
    {
        return GenerateRenderLink(options, "jpg", sign);
    }

    /// <summary>
    /// Generates a URL for a PDF file using the provided options.
    /// </summary>
    /// <param name="options">The options for generating the PDF.</param>
    /// <returns>A render link Url to render a PDF file.</returns>
    public string GeneratePDFUrl(UrlboxOptions options, bool sign = false)
    {
        return GenerateRenderLink(options, "pdf", sign);
    }

    /// <summary>
    /// Generates a Urlbox URL with the specified format.
    /// </summary>
    /// <param name="options">The options for generating the screenshot or PDF.</param>
    /// <param name="format">The format of the output, e.g., "png", "jpg", "pdf".</param>
    /// <returns>A render link URL to render the content.</returns>
    public string GenerateRenderLink(UrlboxOptions options, string format = "png", bool sign = false)
    {
        return renderLinkFactory.GenerateRenderLink(baseUrl, options, format, sign);
    }

    /// <summary>
    /// Generates a Urlbox URL with the specified format.
    /// </summary>
    /// <param name="options">The options for generating the screenshot or PDF.</param>
    /// <param name="format">The format of the output, e.g., "png", "jpg", "pdf".</param>
    /// <returns>A render link URL to render the content.</returns>
    public string GenerateSignedRenderLink(UrlboxOptions options, string format = "png")
    {
        return renderLinkFactory.GenerateRenderLink(baseUrl, options, format, true);
    }

    // ** Status and Validation Methods **

    /// <summary>
    /// A method to get the status of a render from an async request
    /// </summary>
    /// <returns></returns>
    public async Task<AsyncUrlboxResponse> GetStatus(string renderId)
    {
        string statusUrl = $"{baseUrl}{STATUS_ENDPOINT}/{renderId}";

        HttpResponseMessage response = await httpClient.GetAsync(statusUrl);
        if (response.IsSuccessStatusCode)
        {
            string responseData = await response.Content.ReadAsStringAsync();

            var deserializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            AsyncUrlboxResponse? asyncResponse = JsonSerializer.Deserialize<AsyncUrlboxResponse>(responseData, deserializerOptions);

            if (asyncResponse != null)
            {
                return asyncResponse;
            }
        }
        throw new ArgumentException($"Failed to check status of async request: {GetUrlboxErrorMessage(response)}");
    }

    /// <summary>
    /// Verifies a webhook response's x-urlbox-signature header to ensure it came from Urlbox.
    /// Only supports a result from an Async Urlbox request
    /// </summary>
    /// <param name="header">The x-urlbox-signature header.</param>
    /// <param name="content">The content to verify.</param>
    /// <returns>Returns a UrlboxWebhookResponse</returns>
    /// <exception cref="ArgumentException">Thrown when the webhook secret is not set in the Urlbox instance.</exception>
    public UrlboxWebhookResponse VerifyWebhookSignature(string header, string content)
    {
        if (urlboxWebhookValidator is null)
        {
            throw new ArgumentException("Please set your webhook secret in the Urlbox instance before calling this method.");
        }

        bool isValid = urlboxWebhookValidator.VerifyWebhookSignature(header, content);

        if (!isValid)
        {
            throw new System.Exception("Cannot verify that this response came from Urlbox. Double check that you're webhook secret is correct.");
        }

        JsonSerializerOptions deserializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        UrlboxWebhookResponse? urlboxWebhookResponse = JsonSerializer.Deserialize<UrlboxWebhookResponse>(content, deserializerOptions) ??
            throw new System.Exception("Cannot verify that this response came from Urlbox. Response could not be deserialized.");

        return urlboxWebhookResponse;
    }

    // PRIVATE

    /// <summary>
    /// Makes an HTTP POST request to the Urlbox API endpoint and returns the response as a <see cref="UrlboxResponse"/> object.
    /// </summary>
    /// <param name="endpoint">The Urlbox API endpoint to send the request to. Must be either /render/sync or /render/async.</param>
    /// <param name="options">The <see cref="UrlboxOptions"/> object containing the configuration options for the API request.</param>
    /// <returns>A <see cref="AbstractUrlboxResponse"/> object containing the result of the API call, which includes the rendered URL and additional data.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid endpoint is provided or when the request fails with a non-successful response code.</exception>
    /// <remarks>
    /// The method first validates the endpoint, then constructs the request with the provided options, serializing them to JSON using the snake_case naming policy. 
    /// The request is authenticated via a Bearer token, and the response is deserialized from camelCase to PascalCase to fit C# conventions.
    /// </remarks>
    private async Task<AbstractUrlboxResponse> MakeUrlboxPostRequest(string endpoint, object options)
    {
        if (endpoint != SYNC_ENDPOINT && endpoint != ASYNC_ENDPOINT)
        {
            throw new ArgumentException("Endpoint must be one of /render/sync or /render/async.");
        }
        string url = baseUrl + endpoint;
        JsonSerializerOptions serializeOptions = new()
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
            WriteIndented = true
        };

        string optionsAsJson = JsonSerializer.Serialize(options, serializeOptions);

        HttpRequestMessage request = new(HttpMethod.Post, url)
        {
            Content = new StringContent(optionsAsJson, Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Authorization", $"Bearer {secret}");
        var deserializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };

        HttpResponseMessage response = await httpClient.SendAsync(request);

        string responseData = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return endpoint switch
            {
                SYNC_ENDPOINT => JsonSerializer.Deserialize<SyncUrlboxResponse>(responseData, deserializerOptions)
                    ?? throw new System.Exception("Could not deserialize response from Urlbox API."),
                ASYNC_ENDPOINT => JsonSerializer.Deserialize<AsyncUrlboxResponse>(responseData, deserializerOptions)
                    ?? throw new System.Exception("Could not deserialize response from Urlbox API."),
                _ => throw new ArgumentException("Invalid endpoint."),
            };
        }
        else
        {
            throw UrlboxException.FromResponse(responseData, deserializerOptions);
        }
    }

    /// <summary>
    /// Downloads content from the given Urlbox render link and processes it using the provided onSuccess function.
    /// </summary>
    /// <param name="urlboxUrl">The render link Urlbox URL.</param>
    /// <param name="onSuccess">The function to execute when the download is successful.</param>
    /// <returns>The result of the success function.</returns>
    private async Task<string> Download(string urlboxUrl, Func<HttpResponseMessage, Task<string>> onSuccess)
    {
        HttpResponseMessage response = await httpClient.GetAsync(urlboxUrl).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            return await onSuccess(response);
        }
        else
        {
            throw new System.Exception(GetUrlboxErrorMessage(response));
        }
    }
}
