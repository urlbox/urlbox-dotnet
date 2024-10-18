![image](https://user-images.githubusercontent.com/1453680/143582241-f44bd8c6-c242-48f4-8f9a-ed5507948588.png)
# Urlbox .NET Library

The Urlbox .NET package provides easy access to the [Urlbox website screenshot API]("https://urlbox.com/") from your application.

Just initialise the Urlbox class and generate a screenshot of a URL in no time.

Check out our [blog](https://urlbox.com/blog) for more insights on everything screenshots.

## Documentation

See the [Urlbox API Docs](https://urlbox.com/docs/overview). It gives you an exhaustive list of all the options you could pass to our API, including what they do and example usage.

You can also upload to [S3](https://urlbox.com/docs/guides/s3) for more control over your renders, or use [proxies](https://urlbox.com/docs/guides/proxies) for geo-specific sites.

## Requirements

To use this SDK, you need .NET Core 6.0 or later.

We have chosen to maintain compatibility with 6.0 at this time, given its Long-Term Support (LTS) status.
 
## Installation

You can install the SDK via NuGet:

```bash
dotnet add package urlbox.sdk.dotnet
```

## Usage

1. Pull in the Urlbox SDK into the file you're intending to call Urlbox from:

```CS
using Screenshots;
```

2. Create an instance of Urlbox. Your webhook secret is optional, and can be found by visiting Urlbox, then finding your settings->projects->your-project-name:

```CS
Urlbox urlbox = new Urlbox("MY_URLBOX_KEY", "MY_URLBOX_SECRET", "MY_URLBOX_WEBHOOK_SECRET");
```

3. Create an instance of the UrlboxOptions you wish you pass into your render link or sync/async request. Passing a Url or Html is required, but all other options are optional:

```CS
UrlboxOptions optionsUrl = new UrlboxOptions(url: "https://urlbox.com/automated-screenshots/how-performance-cheats-broke-our-website-screenshots");
// OR
UrlboxOptions optionsHtml = new UrlboxOptions(html: "<h1>Hello World!</h1>");

// For a full list of our options, checkout the UrlboxOptions type or our docs.
optionsHtml.ClickAccept = true; // Clicks accept on any cookie banners
optionsHtml.EngineVersion = "latest"; // You could use our latest or stable engine
optionsHtml.UseS3 = true; // Uses your S3 configuration to store your screenshots in your own cloud bucket.
```

4. Pass those options into any one of our render requests. These are:

### `Render()`

Example:

`SyncUrlboxResponse response = await urlbox.Render(options);`

This will take the screenshot and wait for the screenshot to finish before returning you a `SyncUrlboxResponse` with your:

- RenderUrl - This is the Url which has your screenshot stored. It will either be the default Urlbox storage location, or your own S3 bucket if you have it configured.
- Size - This is the size of the screenshot, in bytes.

### `RenderAsync()`

Example:

`AsyncUrlboxResponse response = await urlbox.RenderAsync(options);`

This will send a request to Urlbox to render a screenshot, not waiting for the screenshot to finish. It will return you an `AsyncUrlboxResponse` with:

- Status 
- RenderId - This is the UUID for your screenshot request
- StatusUrl - This is a URL you can poll (run GET requests to) to check if your render has finished and succeeded. Alternatively you can use our webhook feature for a more streamlined approach to /async.

### `GenerateUrlboxUrl()`

Example:

`string url = await urlbox.GenerateUrlboxUrl(options);`

This will generate you a [render link](https://urlbox.com/docs/render-links). This is a link which can be used to make a request to our API and return you the screenshot directly. One useful case for this is as a convenient way for you to embed a Urlbox screenshot in an HTML tag.

---

We also have a number of helper functions:

Note - The 3 format related methods are not an exhaustive list of available formats. Please see the below example and documentation for a full list of available formats to pass into the main GenerateUrlboxUrl() method as an option. All of the below generate [render links](https://urlbox.com/docs/render-links).

`DownloadAsBase64(options)` - Gets a render link, opens it, then downloads the screenshot file as a Base64 string.

`DownloadToFile(options, filePath)` - Gets a render link, opens it, then downloads and stores the screenshot to the given filePath.

`GeneratePNGUrl(options)` - Gets a render link for a screenshot in PNG format.

`GenerateJPEGUrl(options)` - Gets a render link for a screenshot in JPEG format.

`GeneratePDFUrl(options)` - Gets a render link for a screenshot in PDF format.

Helpers Example Usage:

```CS
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screenshots;

namespace MyClass
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

            // Create the options for the request
            var options = new UrlboxOptions(url: "https://urlbox.com/screenshot-behind-login");

            options.ClickAccept = true;
            options.FullPage = true;

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
        }
    }
}
```

## Feedback

Feel free to contact us if you spot a bug or have any suggestions at: `support@urlbox.com` or use our chat function on [our website](https://urlbox.com/).

## Contributing

Want to help? Please follow this process to help us maintain a quality SDK:

1. Ensure that an ISS ticket has been generated for the improvement/feature in question with a descriptive explanation as to the issue/feature.
2. Create a branch from that ticket, and ensure the ISS has that branch assigned to it, and you're assigned to that ISS.
3. Create your solution to the ISS ticket. If you become blocked, you're more than welcome to ask for help at `support@urlbox.com`.
4. Write tests for your solution, and ensure all of the pre-written tests pass.
5. Create a PR with a meaningful description of what you have implemented and how it fixes the ISS.

In order to run the tests, you'll need to set some environment variables in the Urlbox.MsTest project using `dotnet user-secrets init`:

`dotnet user-secrets set "URLBOX_KEY" "<PLACE_YOUR_KEY_HERE>"`
`dotnet user-secrets set "URLBOX_SECRET" "<PLACE_YOUR_SECRET_HERE>"`

These should be your genuine API key and Secret, and will save you accidentally committing them to the branch you're working on.