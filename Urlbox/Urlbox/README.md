[![image](/Urlbox/Urlbox/urlbox-io-graphic.jpg)](https://www.urlbox.com)


***

# The Urlbox .NET SDK

The Urlbox .NET SDK provides easy access to the [Urlbox API](https://urlbox.com/) from your application.

Just initialise Urlbox and generate a screenshot of a URL or HTML in no time.

Check out our [blog](https://urlbox.com/blog) for more insights on everything screenshots and what we're doing.

> **Note:** At Urlbox we make `Renders`. Typically, when we refer to a render here or anywhere else, we are referring to the entire process as a whole of taking your options, performing our magic, and sending back a screenshot your way.

#### Checkout [OneMillionScreenshots](https://onemillionscreenshots.com/) - A site that uses Urlbox to show over 1 million of the web's homepages!
***

# Table Of Contents
 
<!-- TOC -->
* [Documentation](#documentation)
* [Requirements](#requirements)
* [Installation](#installation)
* [Usage](#usage)
  * [Start here](#start-here)
  * [Getting Started - `TakeScreenshot()`](#getting-started---takescreenshot)
  * [Configuring Options](#configuring-options-)
    * [Using the options builder](#using-the-options-builder)
    * [Using the `new` keyword, setting during initialization](#using-the-new-keyword-setting-during-initialization)
    * [Using the `new` keyword, setting after initalization](#using-the-new-keyword-setting-after-initalization)
  * [Render Links - `GenerateRenderLink()`](#render-links---generaterenderlink)
  * [Sync Requests - `Render()`](#sync-requests---render)
  * [Async Requests - `RenderAsync()`](#async-requests---renderasync)
    * [Polling](#polling)
    * [Webhooks](#webhooks)
  * [Dependency Injection](#dependency-injection)
* [Utility Functions](#utility-functions)
    * [`TakePdf(options)`](#takepdfoptions)
    * [`TakeMp4(options)`](#takemp4options)
    * [`TakeFullPage(options)`](#takefullpageoptions)
    * [`TakeMobileScreenshot(options)`](#takemobilescreenshotoptions)
    * [`DownloadAsBase64(options)`](#downloadasbase64options-)
    * [`DownloadToFile(options, filePath)`](#downloadtofileoptions-filepath-)
    * [`GeneratePNGUrl(options)`](#generatepngurloptions-)
    * [`GenerateJPEGUrl(options)`](#generatejpegurloptions-)
    * [`GeneratePDFUrl(options)`](#generatepdfurloptions-)
* [Popular Use Cases](#popular-use-cases)
  * [Extracting Markdown/Metadata/HTML](#extracting-markdownmetadatahtml)
  * [Generating a Screenshot Using a Selector](#generating-a-screenshot-using-a-selector)
  * [Uploading to the cloud via an S3 bucket](#uploading-to-the-cloud-via-an-s3-bucket)
  * [Using a Proxy](#using-a-proxy)
  * [Using Webhooks](#using-webhooks)
    * [1. Visit your Urlbox dashboard, and get your Webhook Secret.](#1-visit-your-urlbox-dashboard-and-get-your-webhook-secret)
    * [2. Create your Urlbox instance in your C# project:](#2-create-your-urlbox-instance-in-your-c-project)
    * [3. Make a request through any of our rendering methods.](#3-make-a-request-through-any-of-our-rendering-methods-)
    * [4. Verify that the webhook comes from Urlbox](#4-verify-that-the-webhook-comes-from-urlbox)
* [API Reference](#api-reference)
  * [Urlbox API Reference](#urlbox-api-reference)
    * [Constructor](#constructor)
    * [Static Methods](#static-methods)
    * [Screenshot and File Generation Methods](#screenshot-and-file-generation-methods)
    * [Download and File Handling Methods](#download-and-file-handling-methods)
    * [URL Generation Methods](#url-generation-methods)
    * [Status and Validation Methods](#status-and-validation-methods)
  * [Feedback](#feedback)
  * [Changelog](#changelog)
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

## Start here

If you've not yet signed up for a trial, please do so by visiting [Urlbox](https://urlbox.com). Once you're in, you'll need to visit your [projects](https://urlbox.com/dashboard/projects) page, and gather your Publishable Key, Secret Key, and Webhook Secret key (if you intend on using webhooks).

With a new account you'll only have one project, so click on it, and you should see something like this:

![The project settings page](projectKeys.png)

Copy over your three keys and place them somewhere safe. They'll be passed into our Urlbox instance in just a moment.

## Getting Started - `TakeScreenshot()`

If you want something super simple, just call our `TakeScreenshot(options)` method with an instance of the UrlboxOptions:

```CS
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlboxSDK; // This is our package

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

            // Create an instance of Urlbox and the Urlbox options you'd like to use
            
            Urlbox urlbox = Urlbox.FromCredentials(apiKey, apiSecret, webhookSecret);
            UrlboxOptions options = Urlbox.Options(url: "https://google.com").Build();
          
            // Take a screenshot
            AsyncUrlboxResponse response = urlbox.TakeScreenshot(options);
            
            // This is the URL destination where you can find your finalized screenshot.
            Console.Writeline(response.RenderUrl); 
        }
    }
}
```

***

## Configuring Options 

Options are simply extra inputs that we use to adapt the way we take the screenshot, or adapt any of the other steps involved in the rendering process.

>**Note:** Almost all of our options are optional. However, you must at least provide a URL or some HTML in your options in order for us to know what we are rendering for you.

You could, for example, change the way the request is made to your desired URL (like using a proxy server, passing in extra headers, an authorization token or some cookies), or change the way the page looks (like injecting Javascript, highlighting words, or making the background a tasteful fuchsia pink). 

There are a few ways to retrieve a screenshot from Urlbox, depending on how and when you need it. You could retrieve it as a [raw file](https://urlbox.com/docs/options#response_type) (using `UrlboxOptions.ResponseType = "binary"` ), or by default, as a JSON
object with its size and location. 

There are a plethora of other options you can use. Checkout the [docs](https://urlbox.com/docs/overview) for more information.

To initialise your urlbox options, we advise using the option builder. Start by calling the static method `Urlbox.Options()` with your url or HTML.

The builder will pre-validate your options on `.Build()`, and allow for a more readable/fluent interface in your code.

### Using the options builder
```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://google.com"
                )
                .Format("pdf")
                .FullPage() // Sets to true with no args
                .Gpu()
                .ResponseType("json")
                .BlockAds()
                .HideCookieBanners()
                .BlockUrls("https://ads.com", "https://trackers.com")
                .Build();

AsyncUrlboxResponse response = urlbox.TakeScreenshot(options);
```

You can alternatively set the Urlbox options with the `new` keyword.

### Using the `new` keyword, setting during initialization
```CS

UrlboxOptions options = new(url: "https://urlbox.com/docs")
{
  Format = "png",
  FullPage = true,
  Gpu = true,
  Retina = true,
  DarkMode = true
};

AsyncUrlboxResponse response = urlbox.TakeScreenshot(options);
```

### Using the `new` keyword, setting after initalization
```CS
UrlboxOptions options = new(url: "https://onemillionscreenshots.com/");
options.FullPage = true;

AsyncUrlboxResponse response = urlbox.TakeScreenshot(options);
```

***

## Render Links - `GenerateRenderLink()`

With Urlbox you can get a screenshot in a number of ways. It may seem a little complex at first, but each method has its purpose.

Take a look at the [section in our docs](https://urlbox.com/docs/api/rest-api-vs-render-links#render-links) which explains the main benefits of using a render link over our `/sync` and `/async` methods.

To get a render link, run the `GenerateRenderLink(options)` method on an instance of Urlbox. Pass in a `UrlboxOptions` instance and you should receive a render link.

Once you have that render link, you're free to embed it anywhere you please. Making a GET request to that render link will synchronously run a render, and return a screenshot. This is particularly handy for embedding into an `<img>` tag.

Here's an example:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://google.com"
                )
                .Format("pdf")
                .Build();

string renderLink = urlbox.GenerateRenderLink(options);
```

## Sync Requests - `Render()`

We have 2 other ways to get a screenshot from Urlbox, `render/sync` and `render/async`. These may be ever so slightly different to the definitions of sync and async that you've heard of in common programming languages, but each serve an important purpose.

Making a request to the [`/sync`](https://urlbox.com/docs/api#create-a-render-synchronously) endpoint means making a request that waits for your screenshot to be taken, and only then returns the response with your finished screenshot.

You can achieve this by using the `render(options)` method within this SDK. It takes the UrlboxOptions, and makes a POST request to the `/sync` endpoint.

Here is an example of its usage:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://google.com"
                )
                .Format("pdf")
                .Build();

SyncUrlboxResponse = urlbox.render(options);
```

If you haven't explicitly asked for a binary response in your options, a 200 response would look like this:

```JSON
{
    // Where the final screenshot is stored -- If you setup S3, it will be your bucket in the URL.
    "renderUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.mp4",
    // The size of the file in bytes
    "size": 272154
}
```

If you find that the kind of screenshot you are taking requires some time, and you don't want your network connection to be open for that long, the `/async` method may be better suited to your needs. 

***

## Async Requests - `RenderAsync()`

Some renders can take some time to complete (think full page screenshots of infinitely scrolling sites, MP4 with retina level quality, or large full page PDF renders).

If you anticipate your request being larger, then we would recommend using the [`/async`](https://urlbox.com/docs/api#create-a-render-asynchronously) endpoint, to reduce your network request time.

Within the SDK you'll find the `renderAsync(options)` method. This method hits the `/async` endpoint.

Here is an example of its usage:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://google.com"
                )
                .Format("pdf")
                .Build();

AsyncUrlboxResponse = urlbox.renderAsync(options);
```

This returns you:

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

You can find out _when_ your async render has been successfully made in two ways:

### Polling

You can [poll](https://en.wikipedia.org/wiki/Polling_(computer_science)) the `statusUrl` endpoint that comes back from the `/async` response via an HTTP GET request. When the render has succeeded, the response from the polling endpoint will include `"status": "succeeded"`, as well as your final render URL. 

You could set up your own polling mechanism to check for this and the renderUrl, though our `TakeScreenshot(options, timeout)` has the polling mechanism built in. The method accepts an optional timeout to tell it exactly when to stop polling. The method will try for 60 seconds by default.

### Webhooks

The other way to find out when your render has succeeded is to use [webhooks](https://urlbox.com/docs/webhooks#using-webhooks). You make a request to Urlbox, and the response gets sent as a POST request to an endpoint of your choosing.

See the [Using Webhooks](#using-webhooks) section in our popular use cases for how to use webhooks with Urlbox in your application.

## Dependency Injection

Using the `new` keyword is often considered a big ❌, because it makes unit testing more difficult, and means defining _how_ an instance of something should be made in many places and not one, violating the Dependency Inversion principle in SOLID.

To avoid this, we recommend that you make your Urlbox instance a singleton, defining how the Urlbox instance should be made once, and injecting it into the classes that are needed.

If you're new to DI and want to implement it, take a look at [this intro from Microsoft](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-basics).

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

### `DownloadAsBase64(options)` 

Gets a render link, runs a GET to it to render your screenshot, then downloads the screenshot file as a Base64 string.

### `DownloadToFile(options, filePath)` 

Gets a render link, runs a GET to it to render your screenshot, then downloads and stores the screenshot to the given filePath.

### `GeneratePNGUrl(options)` 

Gets a render link for a screenshot in PNG format.

### `GenerateJPEGUrl(options)` 

Gets a render link for a screenshot in JPEG format.

### `GeneratePDFUrl(options)` 

Gets a render link for a screenshot in PDF format.

# Popular Use Cases

## Extracting Markdown/Metadata/HTML

In addition to your main render format for your URL/HTML, you can additionally render and save the same screenshot as HTML, Markdown and/or Metadata in the same request.

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://google.com"
                )
                .Format("mp4")
                .SaveMarkdown() // This saves the same URL/HTML's content as a markdown file
                .SaveHtml() // This saves the same URL/HTML's content as its HTML
                .SaveMetadata() // This extracts the metadata, saves it and sends it back in the response.
                .Metadata() // This extracts the metadata from the URL/HTML, and sends it back in the response without saving it to the cloud.
                .Build();

SyncUrlboxResponse = urlbox.render(options);
```

The JSON response for this request would look like this:

```JSON
{
  "renderUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.mp4",
  "size": 1048576,
  "htmlUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.html",
  "metadataUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.json",
  "markdownUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.md",
  "metadata": {
    "title": "Example Page",
    "description": "This is an example of metadata information.",
    "screenshot_date": "2024-11-06T12:34:56Z",
    "file_size": 1048576,
    "mime_type": "image/png"
  }
}
```

When using the screenshot and file generation methods from our SDK like `TakeScreenshot()`, `Render()` or `RenderAsync`, these responses will all be turned into a readable class instance for you.

When downloading metadata, you can opt to either save the metadata, or just return it in the JSON response as above. Our helper method `TakeScreenshotWithMetadata()` will not store the metadata so not produce a URL. It will instead only return the metadata as above.

## Generating a Screenshot Using a Selector

There are times when you don't want to screenshot the entirety of a website. You may want to avoid cropping after taking your screenshot. This is useful in cases where you're looking for something on the page in particular, like images, or a block of text.

You can take a screenshot of only the elements that you wish to using the selector option.

To do this via the SDK, you can call any of our public methods for taking a screenshot, passing in a value to the `UrlboxOptions.Selector`.

Here's an example with our `Render(options)` method:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(url: "https://github.com")
    .Selector(".octicon-mark-github")
    .Build();

SyncUrlboxResponse response = urlbox.Render(options);
```

This will take the ID selector ".octicon-mark-github", and return a screenshot that looks like this:

![](./gh.png)

## Uploading to the cloud via an S3 bucket

For a typical render, we do the storing for you. When you get your final render URL, that screenshot will be stored by us.

You can opt to save the final screenshot to your own cloud provider.

We would _**highly**_ recommend you follow our S3 setup instructions. Setting up a cloud bucket can be tedious at the best of times, so [this](https://urlbox.com/docs/storage/configure-s3) part of our docs can help untangle the process.

The current cloud providers we support are:

- BackBlaze B2
- AWS S3
- Cloudflare R2
- Google Cloud Storage
- Digital Ocean Spaces

Though if there's another cloud provider you would like to use, please try to reach out to us if you're struggling to get setup.

We allow for public CDN hosts, private buckets and buckets with object locking enabled.

Once you've set up your bucket, you can simply add `UrlboxOptions.UseS3 = true` to your options before making your request.

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(url: "https://google.com")
    .UseS3()
    .Build();

SyncUrlboxResponse response = urlbox.Render(options);
```

You'll see that the render URL will include a link to reach the object in your bucket.

## Using a Proxy

Sometimes there are sites only available if your making the request from a particular country. Other times you simply get blocked from a website. Proxies can really help get past these issues, and are quite a similar setup process to uploading to S3.

We have a great piece in our [docs](https://urlbox.com/docs/guides/proxies) to get you started.

Simply pass in the proxy providers' details once you're set up, and we will make the request through that proxy. Here's an example:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(url: "https://google.com")
    .Proxy("http://brd-customer-hl_1a2b3c4d-zone-social_networks:ttpg162fe6e2@brd.superproxy.io:22225")
    .Build();

SyncUrlboxResponse response = urlbox.Render(options);
```

## Using Webhooks

Webhooks are awesome. They save you time, money and headaches, and can quite equally cause just as many setting them up.

Setting up a webhook with Urlbox has some optional steps, but we recommend you take them all, to best secure your product which consumes our API.

### 1. Visit your Urlbox dashboard, and get your Webhook Secret.

Go to your [projects](https://urlbox.com/dashboard/projects) page, select a project (you may only have one if you're just starting out with Urlbox), and copy the webhook secret key.

### 2. Create your Urlbox instance in your C# project:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");
```

### 3. Make a request through any of our rendering methods. 

The most common use case for a webhook is when you need to use the `/async` endpoint to handle a larger render.

If you're developing locally, we would recommend using a service like [ngrok](https://ngrok.com/), and making your webhook URL hit that ngrok endpoint. Ngrok simply exposes a port on your machine to a UUID style domain. You can locally serve your application from that port.

After you've added the endpoint to your application, for example at the endpoint `webhooks/urlbox`, make a request to that endpoint like this:

```CS
static async Task Main()
{
    Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");
    
    UrlboxOptions options = Urlbox.Options(url: "https://google.com")
      .WebhookUrl("https://myapp.com/webhooks/urlbox)
      .Build();

    SyncUrlboxResponse response = urlbox.Render(options);
}
```

### 4. Verify that the webhook comes from Urlbox

Once you have made your request, you should see it come in as a POST request to the endpoint you've made in your app for the webhook. The body should look like this:

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

There will also be our handy header `X-Urlbox-Signature` that looks like this: `t={timestamp},sha256={token}`.

Extract both the header and the content, and pass it into `Urlbox.VerifyWebhookSignature(header, content)`.

Here's an example of verifying the webhook with something (very) basic:

```CS

using System.Text;
using UrlboxSDK;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseHttpsRedirection();

app.MapPost("/webhook/urlbox", async (HttpContext context) =>
{
    using StreamReader stream = new StreamReader(context.Request.Body);

    string header = context.Request.Headers["x-urlbox-signature"];
   
    // Create an instance of Urlbox
    Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");
    
    bool isVerified = urlbox.VerifyWebhookSignature(header, await stream.ReadToEndAsync());

    Console.WriteLine(isVerified);

    if (isVerified)
    {
        return "{\"message\" : \"Woohoo ! This is from Urlbox.\"}";
    }
    else
    {
        return "{\"message\" : \"Uh oh, not verified! \"}";
    }
});

app.Run();

```

# API Reference

Below is a brief description of every publicly available method our SDK provides:

## Urlbox API Reference

### Constructor
- **`Urlbox(string key, string secret, string webhookSecret = null)`**  
  Initializes a new instance of the Urlbox class with the provided API credentials and optional webhook secret.

---

### Static Methods
- **`static Urlbox FromCredentials(string apiKey, string apiSecret, string webhookSecret)`**  
  Creates a new instance of the Urlbox class using the specified API key, secret, and optional webhook secret.

---

### Screenshot and File Generation Methods

- **`Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options);`**
- **`Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options, int timeout);`**  
  Takes a screenshot asynchronously, with an optional timeout for large requests.

- **`Task<AsyncUrlboxResponse> TakePdf(UrlboxOptions options);`**  
  Asynchronously generates a PDF based on the specified options.

- **`Task<AsyncUrlboxResponse> TakeMp4(UrlboxOptions options);`**  
  Generates an MP4 video asynchronously using the specified options.

- **`Task<AsyncUrlboxResponse> TakeFullPageScreenshot(UrlboxOptions options);`**  
  Captures a full-page screenshot asynchronously with the given options.

- **`Task<AsyncUrlboxResponse> TakeMobileScreenshot(UrlboxOptions options);`**  
  Takes a mobile-optimized screenshot asynchronously based on the specified options.

- **`Task<AsyncUrlboxResponse> TakeScreenshotWithMetadata(UrlboxOptions options);`**  
  Asynchronously takes a screenshot and includes metadata in the response.

- **`Task<SyncUrlboxResponse> Render(UrlboxOptions options);`**  
  Sends a synchronous request to generate a render with the provided options, returning a direct response.

- **`Task<AsyncUrlboxResponse> RenderAsync(UrlboxOptions options);`**  
  Sends an asynchronous render request, providing a status URL for polling until completion.

---

### Download and File Handling Methods

- **`Task<string> DownloadAsBase64(UrlboxOptions options, string format = "png");`**  
  Downloads a screenshot as a Base64-encoded string in the specified format.

- **`Task<string> DownloadAsBase64(string urlboxUrl);`**  
  Downloads the screenshot from the provided URL as a Base64-encoded string.

- **`Task<string> DownloadToFile(string urlboxUrl, string filename);`**  
  Downloads a screenshot from the URL and saves it to the specified file path.

- **`Task<string> DownloadToFile(UrlboxOptions options, string filename, string format = "png");`**  
  Generates a screenshot based on options, then downloads and saves it as a file.

---

### URL Generation Methods

- **`string GeneratePNGUrl(UrlboxOptions options);`**  
  Generates a PNG URL based on the specified screenshot options.

- **`string GenerateJPEGUrl(UrlboxOptions options);`**  
  Creates a JPEG URL using the provided rendering options.

- **`string GeneratePDFUrl(UrlboxOptions options);`**  
  Generates a PDF URL for the specified screenshot options.

- **`string GenerateRenderLink(UrlboxOptions options, string format = "png");`**  
  Constructs an Urlbox URL for the specified format and options.

---

### Status and Validation Methods

- **`Task<AsyncUrlboxResponse> GetStatus(string statusUrl);`**  
  Retrieves the current status of an asynchronous render request.

- **`bool VerifyWebhookSignature(string header, string content);`**  
  Verifies that a webhook signature originates from Urlbox using the configured webhook secret.

## Feedback

We hope that the above has given you enough of an understanding to suit your use case.

If you are still struggling, spot a bug, or have any suggestions, feel free to contact us at: `support@urlbox.com` or use our chat function on [our website](https://urlbox.com/).

Get rendering!

## Changelog

- 2.0.0 - Major overhaul - **Non-backward compatible changes included.**
  - Introduced fluent options builder with input validation
  - Introduced options as a typed class
  - Introduced webhook validation logic
  - Upgraded test suite
  - Created interfaces for better Dependency Injection compatibility
  - Introduced post sync and async methods
  - Introduced helper methods for common use cases
  - Overhauled readme including an API reference
  - Introduced logic and classes for side renders (save_html etc)
  - Introduced classes for different response types from urlbox api

Methods in previous versions of this SDK that would accept a Dictionary now take a standardised `UrlboxOptions` type.

- 1.0.2 - Further Updates to readme.
 
- 1.0.1 - Update Readme to replace instances of .io with .com.

- 1.0.0 - First release!
