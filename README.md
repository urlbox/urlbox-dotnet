![image](https://user-images.githubusercontent.com/1453680/143582241-f44bd8c6-c242-48f4-8f9a-ed5507948588.png)
# Urlbox .NET Library

The Urlbox .NET package provides easy access to the [Urlbox website screenshot API]("https://urlbox.com/") from your application.

Just initialise the Urlbox class and generate a screenshot of a URL in no time.

Check out our [blog](https://urlbox.com/blog) for more insights on everything screenshots.

## Documentation

See the [Urlbox API Docs](https://urlbox.com/docs/overview). It gives you an exhaustive list of all the options you could pass to our API, including what they do and example usage.

You can also upload to [S3](https://urlbox.com/docs/guides/s3) for more control over your renders, or use [proxies](https://urlbox.com/docs/guides/proxies) for geo-specific sites.

## Requirements

To use this SDK, you need .NET Core 2.0 or later.
 
## Installation

You can install the SDK via NuGet:

```bash
dotnet add package urlbox.sdk.dotnet
```

## Usage

Pull in the Urlbox SDK with `using Screenshots;`, then create a new Urlbox instance and call any of the following methods:

Note - The 3 format related methods are not an exhaustive list of available formats. Please see the below example and documentation for a full list of available formats to pass into the main GenerateUrlboxUrl() method as an option. All of the below generate [render links](https://urlbox.com/docs/render-links).

`DownloadAsBase64(options)` - Gets a render link, opens it, then downloads the screenshot file as a Base64 string.

`DownloadToFile(options, filePath)` - Gets a render link, opens it, then downloads and stores the screenshot to the given filePath.

`GeneratePNGUrl(options)` - Gets a render link for a screenshot in PNG format.

`GenerateJPEGUrl(options)` - Gets a render link for a screenshot in JPEG format.

`GeneratePDFUrl(options)` - Gets a render link for a screenshot in PDF format.

`GenerateUrlboxUrl(options)` - Gets a render link for a screenshot.

Example Usage:

```CS
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screenshots;

namespace UrlboxTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // We highly recommend storing your Urlbox API key and secret somewhere secure.
            string apiKey = Environment.GetEnvironmentVariable("URLBOX_API_KEY");
            string apiSecret = Environment.GetEnvironmentVariable("URLBOX_API_SECRET");

            // Create an instance of Urlbox
            Urlbox urlbox = new Urlbox(apiKey, apiSecret);

            // Define the options for the screenshot
            var options = new Dictionary<string, object>
            {
                { "url", "https://urlbox.com/screenshot-behind-login" },
            };

            // Download as base64
            string base64Screenshot = await urlbox.DownloadAsBase64(options);
            Console.WriteLine("Screenshot as Base64: " + base64Screenshot);

            // Download to a filepath
            string filePath = "screenshot.png";
            string result = await urlbox.DownloadToFile(options, filePath);
            Console.WriteLine($"Screenshot saved to {filePath}");

            // Generate a PNG render link Url
            string pngUrl = urlbox.GeneratePNGUrl(options);
            Console.WriteLine("Generated PNG URL: " + pngUrl);

            // Generate a PDF render link Url
            string pdfUrl = urlbox.GeneratePDFUrl(options);
            Console.WriteLine("Generated PDF URL: " + pdfUrl);

            // Generate JPEG render link Url
            string jpegUrl = urlbox.GenerateJPEGUrl(options);
            Console.WriteLine("Generated JPEG URL: " + jpegUrl);

            // Define more options for the screenshot, to render different formats
            var optionsWithFormat = new Dictionary<string, object>
            {
                {"url", "https://urlbox.com/screenshot-behind-login"},
                { "format", "png" }, // One of png, jpeg, webp, avif, svg, pdf, html, mp4, webm or md
                { "full_page", true }, // Takes a full page screenshot
            };

            string url = urlbox.GenerateUrlboxUrl(optionsWithFormat);
            Console.WriteLine("Generated URL: " + url);
        }
    }
}
```

We also offer other methods of generating screenshots apart from render links, including POST requests via [async](https://urlbox.com/docs/api#create-a-render-asynchronously) calls, using [webhooks](https://urlbox.com/docs/webhooks#using-webhooks) or [synchronously](https://urlbox.com/docs/api#create-a-render-synchronously).

## Feedback

Feel free to contact us if you spot a bug or have any suggestions at: `support@urlbox.com` or use our chat function on [our website](https://urlbox.com/).
