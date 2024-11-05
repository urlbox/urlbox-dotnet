[![image](/Urlbox/Urlbox/urlbox-io-graphic.jpg)](https://www.urlbox.com)


***

# The Urlbox .NET SDK


The Urlbox .NET SDK provides easy access to the [Urlbox website screenshot API](https://urlbox.com/) from your application.

Just initialise Urlbox and generate a screenshot of a URL or HTML in no time.

Check out our [blog](https://urlbox.com/blog) for more insights on everything screenshots and what we're doing.

> **Note:** At Urlbox we make `Renders`. Typically, when we refer to a render here or anywhere else, we are referring to the entire process as a whole of taking your options, performing our magic, and sending back a screenshot your way.

***

# Table Of Contents

<!-- TOC -->
* [Documentation](#documentation)
* [Requirements](#requirements)
* [Installation](#installation)
* [Usage](#usage)
  * [Start Here - `TakeScreenshot()`](#start-here---takescreenshot)
  * [Options](#options-)
  * [Sync - `Render()`](#sync---render)
  * [Async - `RenderAsync()`](#async---renderasync)
    * [Polling](#polling)
    * [Webhooks](#webhooks)
  * [Generating a Mobile View Screenshot](#generating-a-mobile-view-screenshot)
  * [Generating a Full Page Screenshot](#generating-a-full-page-screenshot)
  * [Generating a Screenshot Using a Selector](#generating-a-screenshot-using-a-selector)
  * [Generating PDFs](#generating-pdfs)
  * [Generating Markdown](#generating-markdown)
  * [Extracting Metadata](#extracting-metadata)
  * [Saving HTML](#saving-html)
  * [Saving other formats alongside your main render format](#saving-other-formats-alongside-your-main-render-format)
  * [Scrolling and Fixed videos (MP4)](#scrolling-and-fixed-videos-mp4)
  * [Uploading to the cloud](#uploading-to-the-cloud)
  * [Using Webhooks](#using-webhooks)
  * [Using a Proxy](#using-a-proxy)
* [API Reference](#api-reference)
  * [Constructor](#constructor)
  * [Static Methods](#static-methods)
  * [Screenshot and File Generation Methods](#screenshot-and-file-generation-methods)
  * [Download and File Handling Methods](#download-and-file-handling-methods)
  * [URL Generation Methods](#url-generation-methods)
  * [Status and Validation Methods](#status-and-validation-methods)
  * [Feedback](#feedback)
<!-- TOC -->

***

# Documentation

See the [Urlbox API Docs](https://urlbox.com/docs/overview). It includes an exhaustive list of all the options you could pass to our API, including what they do and example usage.

We also have guides for how to set up uploading your final render to your own [S3](https://urlbox.com/docs/guides/s3) bucket, or use [proxies](https://urlbox.com/docs/guides/proxies) for geo-specific sites.

# Requirements

To use this SDK, you need .NET Core 2.0 or later.
 

# Installation

You can install the SDK via NuGet:

```bash
dotnet add package urlbox.sdk.dotnet
```

# Usage

## Start Here - `TakeScreenshot()`


If you want something super simple, just call our `TakeScreenshot(options)` method with an instance of the UrlboxOptions:

```CS
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screenshots; // This is our package

namespace MyNamespace
{
    class Program
    {
        static async Task Main()
        {
            // We highly recommend storing your Urlbox API key and secret somewhere secure.
            
            string apiKey = Environment.GetEnvironmentVariable("URLBOX_API_KEY");
            string apiSecret = Environment.GetEnvironmentVariable("URLBOX_API_SECRET");
            string webhookSecret = Environment.GetEnvironmentVariable("URLBOX_WEBHOOK_SECRET");

            // Create an instance of Urlbox and the options you want to pass in
            
            Urlbox urlbox = new(apiKey, apiSecret, webhookSecret);
            UrlboxOptions options = new(url: "https://onemillionscreenshots.com/");

            AsyncUrlboxResponse response = urlbox.TakeScreenshot(options);

            Console.Writeline(response.RenderUrl); // This is the URL destination where you can find your final screenshot.
        }
    }
}
```

***

## Options 

Options are simply extra inputs that we use to adapt the way we take the screenshot, or adapt any of the other steps involved in the rendering process.

>**Note:** Almost all of our options are optional. However, you must at least provide a URL or some HTML in your options in order for us to know what we are screenshotting for you.

You could, for example, change the way the request is made to your desired URL (like using a proxy server, passing in extra headers, an authorization token or some cookies), or change the way the page looks (EG injecting Javascript, highlighting words, or making the background a tasteful fuchsia pink). 

There are a few ways to retrieve a screenshot from Urlbox, depending on when and how you need it. You could retrieve it as a [raw file](https://urlbox.com/docs/options#response_type) (using `UrlboxOptions.ResponseType = "binary"` ), or by default as a JSON
object with its size and location. 

There are a plethora of other options you can use. Checkout the [docs](https://urlbox.com/docs/overview) for more information.

Here's an example of setting some options:

```CS
Urlbox urlbox = new("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = new(url: "https://urlbox.com/docs")
{
  Format = "png",
  FullPage = true,
  Gpu = true,
  Retina = true,
  DarkMode = true
};

// OR

UrlboxOptions options = new(url: "https://onemillionscreenshots.com/");
options.FullPage = true;

AsyncUrlboxResponse response = urlbox.TakeScreenshot(options);
```

***

## Sync - `Render()`

We have 2 endpoints for getting a screenshot from Urlbox, `render/sync` and `render/async`. These may be ever so slightly different to the definitions of sync and async that you've heard of in common programming languages, but each serve an important purpose, saving you time and headaches.

Making a request to the [`/sync`](https://urlbox.com/docs/api#create-a-render-synchronously) endpoint means making a request that waits for your screenshot to be taken, and only then returns the response with your finished screenshot.

Within this SDK you'll find the `render(options)` method. It takes the UrlboxOptions, and makes a POST request to this endpoint.

If you haven't explicitly asked for a binary response in your options, a 200 response would look something like:

```JSON
{
    // Where the final screenshot is stored -- If you setup S3, it will be your bucket in the URL.
    "renderUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.mp4",
    // The size of the file in bytes
    "size": 272154
}
```

If you find that the kind of screenshot you are taking requires some time, and you don't want an network connection to be open for that long, or you'd just rather not wait for it, the `/async` method may be better suited to your needs. 

***

## Async - `RenderAsync()`

Some renders can take some time to complete (think full page screenshots of infinitely scrolling sites, MP4 with retina level quality, or large full page PDF renders).

If you anticipate your request being larger, then we would recommend using the [`/async`](https://urlbox.com/docs/api#create-a-render-asynchronously) endpoint, to reduce your network request time.

Within the SDK you'll find the `renderAsync(options)` method. This method hits the async endpoint, and returns you something like this:

```JSON
{
    // When this is "succeeded", your render will be ready
    "status": "created",
    // This is your unique render id
    "renderId": "fe7af5df-80e7-4b38-973a-005ebf06dabb", 
    // Make a GET to this to find out if your render is ready
    "statusUrl": "https://api.urlbox.com/v1/render/fe7af5df-80e7-4b38-973a-005ebf06dabb"
}
```

You can find out _when_ your render is ready to view in two ways:

### Polling

You can [poll](https://en.wikipedia.org/wiki/Polling_(computer_science)) the `statusUrl` endpoint that comes back from the `/async` response via an HTTP GET request. When the render has succeeded, the response from the polling endpoint will include `"status": "succeeded"`, as well as your final render URL. 

You could set up your own polling mechanism to check for this and the renderUrl, though our `TakeScreenshot(options, timeout)` has the polling mechanism built in. The method accepts an optional timeout to tell it exactly when to stop polling. The method will by default try for 60 seconds.

### Webhooks

The other way to find out when your render has succeeded is to use [webhooks](https://urlbox.com/docs/webhooks#using-webhooks).

Urlbox has a neat option. You can pass a webhook URL which you expose in your app, and Urlbox will send the finalised response to it. And when you do receive it, you can use this SDK to verify that the request did indeed come from Urlbox.

Make sure you've setup your Urlbox instance with your webhook secret, provided in your project page accessible with the Urlbox [Dashboard](https://urlbox.com/dashboard/projects).

***
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


## Feedback

Feel free to contact us if you spot a bug or have any suggestions at: `support@urlbox.com` or use our chat function on [our website](https://urlbox.com/).
