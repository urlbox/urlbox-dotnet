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
  * [Getting Started - `TakeScreenshot()`](#getting-started---takescreenshot)
  * [Configuring Options](#configuring-options-)
  * [Sync Requests - `Render()`](#sync-requests---render)
  * [Async Requests - `RenderAsync()`](#async-requests---renderasync)
    * [Polling](#polling)
    * [Webhooks](#webhooks)
* [Utility Functions](#utility-functions)
    * [`TakePdf(options)`](#takepdfoptions)
    * [`TakeMp4(options)`](#takemp4options)
    * [`TakeFullPage(options)`](#takefullpageoptions)
    * [`TakeMobileScreenshot(options)`](#takemobilescreenshotoptions)
* [Popular Use Cases](#popular-use-cases)
  * [Extracting Markdown/Metadata/HTML](#extracting-markdownmetadatahtml)
    * [`SaveMarkdown = true` - This saves the same URL/HTML's content as a markdown file](#savemarkdown--true---this-saves-the-same-urlhtmls-content-as-a-markdown-file)
    * [`SaveHtml = true` - This saves the same URL/HTML's content as its HTML](#savehtml--true---this-saves-the-same-urlhtmls-content-as-its-html)
    * [`SaveMetatada = true` - This extracts the metadata, saves it and sends it back in the response.](#savemetatada--true---this-extracts-the-metadata-saves-it-and-sends-it-back-in-the-response)
    * [`Metatada = true` - This extracts the metadata from the URL/HTML, and sends it back in the response without saving.](#metatada--true---this-extracts-the-metadata-from-the-urlhtml-and-sends-it-back-in-the-response-without-saving)
  * [Generating a Screenshot Using a Selector](#generating-a-screenshot-using-a-selector)
  * [Uploading to the cloud via an S3 bucket](#uploading-to-the-cloud-via-an-s3-bucket)
  * [Using a Proxy](#using-a-proxy)
  * [Using Webhooks](#using-webhooks)
    * [1. Visit your Urlbox dashboard, and get your Webhook Secret.](#1-visit-your-urlbox-dashboard-and-get-your-webhook-secret)
    * [2. Create your Urlbox instance in your C# project:](#2-create-your-urlbox-instance-in-your-c-project)
    * [3. Make a request through any of our screenshotting methods.](#3-make-a-request-through-any-of-our-screenshotting-methods-)
    * [4. Verify that the webhook comes from Urlbox](#4-verify-that-the-webhook-comes-from-urlbox)
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

## Getting Started - `TakeScreenshot()`

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

## Configuring Options 

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

## Sync Requests - `Render()`

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

## Async Requests - `RenderAsync()`

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

Make sure you've set up your Urlbox instance with your webhook secret, provided in your project page accessible with the Urlbox [Dashboard](https://urlbox.com/dashboard/projects).

***

# Utility Functions

To make capturing and rendering screenshots even simpler, we’ve created several specialized methods for common scenarios. Use these methods to quickly generate specific types of screenshots or files based on your needs:

### `TakePdf(options)`
Convert any URL or HTML into a PDF.

### `TakeMp4(options)`
Turn any URL or HTML into an MP4 video. For a scrolling effect over the entire page, set `FullPage = true` to capture the full length of the content.

### `TakeFullPage(options)`
Capture a full-page screenshot of a website, scrolling through the entire page.

### `TakeMobileScreenshot(options)`
Render a screenshot that simulates a mobile device view.

# Popular Use Cases

## Extracting Markdown/Metadata/HTML

In addition to your main render format for your URL/HTML, you can additionally render and save the same render as HTML, Markdown and Metadata in the same request.

Each of the following will return a separate URL where the format is stored.

### `SaveMarkdown = true` - This saves the same URL/HTML's content as a markdown file
### `SaveHtml = true` - This saves the same URL/HTML's content as its HTML
### `SaveMetatada = true` - This extracts the metadata, saves it and sends it back in the response.
### `Metatada = true` - This extracts the metadata from the URL/HTML, and sends it back in the response without saving.

The JSON response would look something like:

```JSON
{
  "renderUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.mp4",
  "size": 1048576,
  "htmlUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.html",
  "mhtmlUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.mhtml",
  "metadataUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.json",
  "markdownUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.markdown",
  "metadata": {
    "title": "Example Page",
    "description": "This is an example of metadata information.",
    "screenshot_date": "2024-11-06T12:34:56Z",
    "file_size": 1048576,
    "mime_type": "image/png"
  }
}
```

Using the screenshot and file generation methods from our SDK like `TakeScreenshot()`, `Render()` or `RenderAsync`, these responses will all be turned into a type for you. See the API reference below for an explanation of all the types in this SDK.

When downloading metadata, you can opt to either save the metadata, or just return it in the JSON response as above. Our helper method `TakeScreenshotWithMetadata()` will not store the metadata so not produce a URL. It will instead only return the metadata field as above, turned into metadata type.

## Generating a Screenshot Using a Selector

There are times when you don't want to screenshot the entirety of a website. You may want to avoid cropping after taking your screenshot. This is useful in cases where you're looking for something on the page in particular, like images, or a block of text.

You can take a screenshot of only the elements that you wish to using the selector option.

To do this via the SDK, you can call any of our public methods for taking a screenshot, passing in a value to the `UrlboxOptions.Selector`.

Here's an example with our `Render(options)` method:

```CS
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screenshots;

namespace MyNamespace
{
    class Program
    {
        static async Task Main()
        {
            // Create an instance of Urlbox and the options you want to pass in
            Urlbox urlbox = new("api_key", "api_secret");

            UrlboxOptions options = new(url: "https://github.com")
            {
              Selector = ".octicon-mark-github"
            };

            SyncUrlboxResponse response = urlbox.Render(options);

            Console.Writeline(response.RenderUrl);
        }
    }
}
```

This will take the ID selector ".octicon-mark-github", and return a screenshot that looks like this:

![](32x32 Image.png)

## Uploading to the cloud via an S3 bucket

For a typical render, we do the storing for you. When you get your final render URL, that screenshot will be stored by us.

You can opt, whether for security, control, compliance or fun, to save the final screenshot to your own cloud provider.

We would _**highly**_ recommend you follow our S3 setup instructions. Setting up a cloud bucket can be tedious at the best of times, so [this](https://urlbox.com/docs/storage/configure-s3) part of our docs can help untangle the process.

The current cloud providers we support are:

- BackBlaze B2
- AWS S3
- Cloudflare R2
- Google Cloud Storage
- Digital Ocean Spaces

Though if there's another cloud provider you use, you're more than willing to reach out to us if you're struggling to get setup.

Once you've set up your bucket, you can simply add `UrlboxOptions.UseS3 = true` to your options before making your request.

```CS
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screenshots;

namespace MyNamespace
{
    class Program
    {
        static async Task Main()
        {
            // Create an instance of Urlbox and the options you want to pass in
            Urlbox urlbox = new("api_key", "api_secret");

            UrlboxOptions options = new(url: "https://google.com")
            {
              UseS3 = true
            };

            SyncUrlboxResponse response = urlbox.Render(options);

            Console.Writeline(response.RenderUrl);
        }
    }
}
```

You'll see that the render URL will include a link to reach the object in your bucket.

## Using a Proxy

Sometimes there are sites only available if your IP address is from a particular country. Other times you simply get blocked from a website. Proxies can really help get past these issues, and are quite a similar setup process to uploading to S3.

We have a great piece in our [docs](https://urlbox.com/docs/guides/proxies) to get you started.

Simply pass in the proxy providers' details once you're set up, and we will make the request through that proxy. Here's an example:

```CS
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screenshots;

namespace MyNamespace
{
    class Program
    {
        static async Task Main()
        {
            // Create an instance of Urlbox and the options you want to pass in
            Urlbox urlbox = new("api_key", "api_secret");

            UrlboxOptions options = new(url: "https://google.com")
            {
              Proxy = "http://brd-customer-hl_1a2b3c4d-zone-social_networks:ttpg162fe6e2@brd.superproxy.io:22225"
            };

            SyncUrlboxResponse response = urlbox.Render(options);

            Console.Writeline(response.RenderUrl);
        }
    }
}
```

## Using Webhooks

Webhooks are awesome. They save you time, money and headaches, and can quite equally cause just as many setting them up.

Setting up a webhook with Urlbox has some optional steps, but we recommend you take them all, to best secure your product which consumes our API.

### 1. Visit your Urlbox dashboard, and get your Webhook Secret.

Go to your [projects](https://urlbox.com/dashboard/projects) page, select a project (you may only have one if you're just starting out with Urlbox), and copy the webhook secret key.

### 2. Create your Urlbox instance in your C# project:

```CS
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screenshots;

namespace MyNamespace
{
    class Program
    {
        static async Task Main()
        {
            Urlbox urlbox = new("api_key", "api_secret", "PLACE_WEBHOOK_SECRET_HERE");
        }
    }
}
```

### 3. Make a request through any of our screenshotting methods. 

The most common use case for a webhook is when you need to use the `/async` endpoint to handle a larger render.

After you've added the endpoint to your application, for example at the endpoint `webhooks/urlbox`, make a request to that endpoint like this:

If you're developing locally, we would recommend using a service like [ngrok](https://ngrok.com/), and making your webhook URL hit that ngrok endpoint. Ngrok simply exposes a port on your machine to a UUID style domain, which is handy because it means you don't have to push your webhook changes to one of your live/staging environments just to test them out.

Remember to assign the `UrlboxOptions.WebhookUrl`: 

```CS
static async Task Main()
{
    Urlbox urlbox = new("api_key", "api_secret", "PLACE_WEBHOOK_SECRET_HERE");
    
    UrlboxOptions options = new(url: "https://github.com")
    {
      // You can use any path your app accepts, this is just an example.
      WebhookUrl = "https://myapp.com/webhooks/urlbox
    };

    SyncUrlboxResponse response = urlbox.Render(options);
}
```

### 4. Verify that the webhook comes from Urlbox

Once you have made your request, poof! it's gone. You should see it come in as a POST request. The body should look something like:

```JSON
{
  "event": "render.succeeded",
  "renderId": "19a59ab6-a5aa-4cde-86cb-d2b23302fd84",
  "result": {
    "renderUrl": "https://renders.urlbox.com/urlbox1/renders/6215a3df94d7588f7d910513/2024/1/11/19a59ab6-a5aa-4cde-86cb-d2b23302fd84.png",
    "size": 34097
  },
  "meta": {
    "startTime": "2024-01-11T17:49:18.593Z",
    "endTime": "2024-01-11T17:49:21.103Z"
  }
}
```

There will also be our handy header `X-Urlbox-Signature` which will look something like `t={timestamp},sha256={token}`.

Extract both the header and the content, and simply pass it into `Urlbox.VerifyWebhookSignature(header, content)`.

Here's an example with something (very) basic:

```CS

using System.Text;
using Screenshots;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseHttpsRedirection();

app.MapPost("/webhook/urlbox", async (HttpContext context) =>
{
    using StreamReader stream = new StreamReader(context.Request.Body);

    string header = context.Request.Headers["x-urlbox-signature"];

    // Your Urlbox credentials
    string apiKey = "MY_URLBOX_KEY";
    string apiSecret = "MY_URLBOX_SECRET";
    string webookSecret = "MY_URLBOX_WEBHOOK_SECRET";

    // Create an instance of Urlbox
    Urlbox urlbox = new Urlbox(apiKey, apiSecret, webookSecret);

    bool isVerified = urlbox.VerifyWebhookSignature(header, await stream.ReadToEndAsync());

    Console.WriteLine(isVerified);

    if (isVerified)
    {
        return "{\"message\" : \"Woohoo ! This is verified.\"}";
    }
    else
    {
        return "{\"message\" : \"NOT VERIFIED \"}";
    }

});

app.Run();

```
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
