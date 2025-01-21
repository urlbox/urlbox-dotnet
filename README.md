[![image](Images/urlbox-graphic.jpg)](https://www.urlbox.com)

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
    * [What to do if an option isn't available in the builder](#what-to-do-if-an-option-isnt-available-in-the-builder)
  * [Render Links - `GenerateRenderLink()`](#render-links---generaterenderlink)
  * [Sync Requests - `Render()`](#sync-requests---render)
  * [Async Requests - `RenderAsync()`](#async-requests---renderasync)
    * [Polling](#polling)
    * [Webhooks](#webhooks)
  * [Handling Errors](#handling-errors)
  * [Dependency Injection](#dependency-injection)
* [Utility Functions](#utility-functions)
    * [`TakeScreenshot(options)`](#takescreenshotoptions)
    * [`TakePdf(options)`](#takepdfoptions)
    * [`TakeMp4(options)`](#takemp4options)
    * [`TakeScreenshotWithMetadata(options)`](#takescreenshotwithmetadataoptions)
    * [`ExtractMetadata(options)`](#extractmetadataoptions)
    * [`ExtractMarkdown(options)`](#extractmarkdownoptions)
    * [`ExtractHtml(options)`](#extracthtmloptions)
    * [`ExtractMhtml(options)`](#extractmhtmloptions)
    * [`DownloadAsBase64(options)`](#downloadasbase64options-)
    * [`DownloadToFile(options, filePath)`](#downloadtofileoptions-filepath-)
    * [`GeneratePNGUrl(options)`](#generatepngurloptions-)
    * [`GenerateJPEGUrl(options)`](#generatejpegurloptions-)
    * [`GeneratePDFUrl(options)`](#generatepdfurloptions-)
* [Popular Use Cases](#popular-use-cases)
  * [Taking a Full Page Screenshot](#taking-a-full-page-screenshot)
    * [Example MP4 (Full Page)](#example-mp4--full-page-)
  * [Taking a Mobile view screenshot](#taking-a-mobile-view-screenshot)
  * [Failing a request on 4XX-5XX](#failing-a-request-on-4xx-5xx)
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
    * [Response Classes](#response-classes)
      * [`SyncUrlboxResponse`](#syncurlboxresponse)
      * [`AsyncUrlboxResponse`](#asyncurlboxresponse)
      * [`WebhookUrlboxResponse`](#webhookurlboxresponse)
      * [`UrlboxException`](#urlboxexception)
      * [`UrlboxMetadata`](#urlboxmetadata)
    * [Available Enums](#available-enums)
  * [Examples](#examples)
    * [Example HTML](#example-html)
    * [Example PDF](#example-pdf)
    * [Example PDF Highlighting](#example-pdf-highlighting)
    * [Example PNG injecting Javascript](#example-png-injecting-javascript)
  * [Feedback](#feedback)
  * [Changelog](#changelog)
<!-- TOC -->

***

# Documentation

See [here](https://urlbox.com/docs/overview) for the Urlbox API Docs. It includes an exhaustive list of all the options you could pass to our API, including what they do and example usage.

We also have guides for how to set up uploading your final render to your own [S3](https://urlbox.com/docs/guides/s3) bucket, or use [proxies](https://urlbox.com/docs/guides/proxies) for geo-specific sites.

# Requirements

To use this SDK, you need .NET Core 6.0 or later.

# Installation

Nuget:

```bash
dotnet add package urlbox.sdk.dotnet
```

# Usage

## Start here

Visit [Urlbox](https://urlbox.com) to sign up for a trial. You'll need to visit your [projects](https://urlbox.com/dashboard/projects) page, and gather your Publishable Key, Secret Key, and Webhook Secret key (if you intend on using webhooks).

With a new account you'll only have one project, so visit the project configuration page for it, where you should see something like this:

![The project settings page](Images/projectKeys.png)

## Getting Started - `TakeScreenshot()`

If you want something super simple, initialize an instance of Urlbox with the above credentials, then call our `TakeScreenshot(options)` method with the options of your choosing:

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
            // Use the builder pattern for fluent options
            UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com").Build();
          
            // Take a screenshot - The default format is PNG
            AsyncUrlboxResponse response = await urlbox.TakeScreenshot(options);
            
            // This is the URL destination where you can find your finalized render.
            Console.Writeline(response.RenderUrl); 
        }
    }
}
```

If you use the above with your own keys, it will give you back an object with a `renderUrl`. Making a GET request to that renderUrl will give you back a PNG back like this:

![](../Images/urlbox-png.png)

***

## Configuring Options 

Passing options are where the magic comes in. Options are simply extra inputs that we use to adapt the way we take the screenshot, or adapt any of the other steps involved in the rendering process.

>**Note:** Almost all of our options are optional. However, you must at least provide a URL or some HTML in your options in order for us to know what we are rendering for you.

You could, for example, change the way the request is made to your desired URL (like using a proxy server, passing in extra headers, an authorization token or some cookies), or change the way the page looks (like injecting Javascript, highlighting words, or making the background a tasteful fuchsia pink). 

There are a few ways to retrieve a screenshot from Urlbox, depending on how and when you need it. You could retrieve it as a [raw file](https://urlbox.com/docs/options#response_type) (using `UrlboxOptions.ResponseType(ResponseType.Binary)` ), or by default, as a JSON object with its size and stored location. 

There are a plethora of other options you can use. Checkout the [docs](https://urlbox.com/docs/overview) for more information.

To initialise your urlbox options, we advise using the option builder. Start by calling the static method `Urlbox.Options()` with the URL or HTML you want to screenshot.

The builder will validate your options on `.Build()`, and allow for a more readable/fluent interface in your code.

### Using the options builder
```CS
using UrlboxSDK;
using UrlboxSDK.Options.Resource;
using UrlboxSDK.Response.Resource;

Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://urlbox.com"
                )
                // Any Bool option sets to true when called with no arguments
                .FullPage()
                .Cookie("some=cookie", "someother=cookie")
                .Gpu()
                // Enumerables can be accessed/imported by their name:
                .ResponseType(ResponseType.Json)
                .BlockAds()
                .HideCookieBanners()
                .BlockUrls("https://ads.com", "https://trackers.com")
                .Build();

AsyncUrlboxResponse response = await urlbox.TakeScreenshot(options);

Console.WriteLine(response.Status);
Console.WriteLine(response.RenderUrl);
```

You can alternatively set the Urlbox options with the `new` keyword.

### Using the `new` keyword, setting during initialization

We advise against using the `new` keyword. If you would like to anyway, here's an example:

```CS
UrlboxOptions options = new(url: "https://urlbox.com")
{
    Format = Format.Pdf,
    Gpu = true,
    Retina = true,
    DarkMode = true
};

// Or set them after init:
options.FullPage = true;

AsyncUrlboxResponse response = await urlbox.TakeScreenshot(options);
```

### What to do if an option isn't available in the builder

Our [latest](https://urlbox.com/docs/options#engine_version) engine is updated regularly, including new options which are released to better help you render screenshots.

If you can't find an option within the builder, because our SDK isn't yet in sync with any latest changes, please do use our overloads for `render` and `renderAsync` which take an `IDictionary<string, object>` instead of a `UrlboxOptions` type.

Here's an example:

```CS
IDictionary<string, object> options = new Dictionary<string, object>
    {
        { "click_accept", true },
        { "url", "https://urlbox.com" }
        { "theOption", "YouCouldntFind" }
    };
SyncUrlboxResponse response = await urlbox.Render(options);

Console.WriteLine(response);
```
Please Bear in mind that this won't have the benefit of pre-validation.

***

## Render Links - `GenerateRenderLink()`

With Urlbox you can get a screenshot in a number of ways. It may seem a little complex at first, but each method has its purpose.

Take a look at the [section in our docs](https://urlbox.com/docs/api/rest-api-vs-render-links#render-links) which explains the main benefits of using a render link over our `/sync` and `/async` methods.

To get a render link, run the `GenerateRenderLink(options)` with your options.

Once you have that render link, you're free to embed it anywhere you please. Make a GET request to that render link, and it will synchronously run a render, and return a screenshot. This is particularly handy for embedding into an `<img>` tag.

The method will, by default, sign the render link, for enhanced security. You can opt out of this by passing `urlbox.GenerateRenderLink(options, sign: false);`

Here's an example:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://bbc.com"
                )
                .Format(Format.Pdf)
                .Build();

string renderLink = urlbox.GenerateRenderLink(options, sign: true);

Console.WriteLine(renderLink);
```

## Sync Requests - `Render()`

We have 2 other ways to get a screenshot from Urlbox, `render/sync` and `render/async`.

Making a request to the [`/sync`](https://urlbox.com/docs/api#create-a-render-synchronously) endpoint means making a request that waits for your screenshot to be taken, and only then returns the response with your finished screenshot. You can achieve this by using the main `Render(options)` method.

Here is an example:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://youtube.com"
                )
                .Format(Format.Pdf)
                .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```

If you haven't explicitly asked for a binary response in your options, you'll get a JSON 200 response like this:

```JSON
{
    # Where the final screenshot is stored -- If you setup S3, it will be your bucket name / cdn host in the URL.
    "renderUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.mp4",
    # The size of the file in bytes
    "size": 272154
}
```

If you find that the kind of screenshot you are taking requires some time, and you don't want your network connection to be open for that long, the `/async` method may be better suited to your needs. Our `TakeScreenshot()` method already implements a polling mechanism using the `/async` endpoint and status checks, so you don't have to set one up yourself!

***

## Async Requests - `RenderAsync()`

Some renders can take some time to complete (think full page screenshots of infinitely scrolling sites, MP4 with retina level quality, or large full page PDF renders).

If you anticipate your request being larger, then we would recommend using the [`/async`](https://urlbox.com/docs/api#create-a-render-asynchronously) endpoint by calling the `RenderAsync(options)` method or `TakeScreenshot(options)`.

Here is an example of its usage:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://google.com"
                )
                .Format(Format.Pdf)
                .Build();

AsyncUrlboxResponse = await urlbox.RenderAsync(options);
```

This returns you:

```JSON
{
    # When this is "succeeded", your render will be ready
    "status": "created",
    # This is your unique render id
    "renderId": "fe7af5df-80e7-4b38-973a-005ebf06dabb", 
    # Make a GET to this to find out if your render is ready
    "statusUrl": "https://api.urlbox.com/v1/render/fe7af5df-80e7-4b38-973a-005ebf06dabb"
}
```

You can find out _when_ your async render has been successfully made in two ways:

### Polling

You can [poll](https://en.wikipedia.org/wiki/Polling_(computer_science)) the `statusUrl` endpoint that comes back from the `/async` response via a GET request. The response from that status URL will include `"status": "succeeded"` when finished, as well as your final render URL.

Use `TakeScreenshot()` to use our `/async` endpoint with a pre-built polling mechanism. The method will try for 60 seconds by default with an optional timeout.

### Webhooks

You can also use [webhooks](https://urlbox.com/docs/webhooks#using-webhooks) to tell you when your render is ready. Make a request to Urlbox, and we send the response as a POST request to an endpoint of your choosing.

See the [Using Webhooks](#using-webhooks) section of these docs in for how to use webhooks with Urlbox in your application.

## Handling Errors

The SDK deserializes our API errors for you into an Exception class.

The UrlboxException gives you some useful data. Here's an example:

```CS
Urlbox urlbox = new(apiKey, apiSecret);

UrlboxOptions options = Urlbox.Options(
        url: "https://notaresolvableurlbox.com"
    )
    .Build();

try
{
    AsyncUrlboxResponse response = await urlbox.TakeScreenshot(options);
}
catch (UrlboxException exception)
{
    Console.WriteLine(exception.Message); // EG Invalid options, please check errors
    Console.WriteLine(exception.Code); // EG InvalidOptions
    Console.WriteLine(exception.Errors); // EG {"url":["error resolving URL - ENOTFOUND notresolvableurlbox.com"]}
    Console.WriteLine(exception.RequestId); // EG 06u6e285-ahd3-45vc-ac8c-36b95e6c15b5 
}
```

The `Code` property will typically result in one of [these](https://urlbox.com/docs/api#error-codes). We're adding to this consistently to make you're error handling experience more streamlined.

Got an unexpected 4XX or 5XX? You can ensure renders fail and don't count toward your render count for [non-2XX responses](#failing-a-request-on-4xx-5xx).

## Dependency Injection

We've set up an extension for DI. When you're configuring your DI you can run `services.AddUrlbox()` to define the Urlbox instance once. Here's a simple ASP.net app:

```CS
using UrlboxSDK.DI.Extension;
using UrlboxSDK;
using UrlboxSDK.Response.Resource;

var builder = WebApplication.CreateBuilder(args);

// Add The Urlbox service to the service container
builder.Services.AddUrlbox(options =>
{
    options.Key = "YOUR_API_KEY";
    options.Secret = "YOUR_SECRET";
    options.WebhookSecret = "YOUR-WEBHOOK-SECRET"; // Optional
    options.BaseUrl = "https://api-eu.urlbox.com";    // Optional
});

var app = builder.Build();

app.UseHttpsRedirection();

// Urlbox gets injected from service container by reference to its interface
app.MapGet("/screenshot", async (HttpContext context, IUrlbox urlbox) =>
{
    var options = Urlbox.Options(url: "https://example.com").Build();
    try
    {
        AsyncUrlboxResponse response = await urlbox.TakeScreenshot(options);
        return Results.Json(new { message = "Screenshot generated!", response });
    }
    catch (Exception ex)
    {
        return Results.Json(new { message = "Failed to generate screenshot", error = ex.Message });
    }
});

app.Run();
```

***

# Utility Functions

To make capturing and rendering screenshots even simpler, we’ve created several methods for common scenarios. Use these methods to quickly generate specific types of screenshots or files based on your needs:

### `TakeScreenshot(options)`
Our simplest method to take a screenshot. Uses the `/async` Urlbox endpoint, and polls until the render is ready to reduce the time network requests stay open.

### `TakePdf(options)`
Convert any URL or HTML into a PDF.

### `TakeMp4(options)`
Turn any URL or HTML into an MP4 video. For a scrolling effect over the entire page, set `FullPage = true` to capture the full length of the content.

### `TakeScreenshotWithMetadata(options)`
Takes a screenshot of any URL or HTML, bringing back a [UrlboxMetadata](#urlboxmetadata) object too with more information about the site.

### `ExtractMetadata(options)`
Takes a screenshot of any URL or HTML, but extracts only the metadata from the render. Useful when you only need the `UrlboxMetadata` object from the render.

### `ExtractMarkdown(options)`
Takes a screenshot of any URL or HTML, downloads it and gives back the extracted markdown file as a string.

### `ExtractHtml(options)`
Takes a screenshot of any URL or HTML, downloads it and gives back the extracted HTML file as a string.

### `ExtractMhtml(options)`
Takes a screenshot of any URL or HTML, downloads it and gives back the extracted MHTML file as a string.

### `DownloadAsBase64(options)` 
Gets a render link, runs a GET to that link to render your screenshot, then downloads the screenshot file as a Base64 string.

### `DownloadToFile(options, filePath)` 
Gets a render link, runs a GET to that link to render your screenshot, then downloads and stores the screenshot to the given filePath.

### `GeneratePNGUrl(options)` 
Gets a render link for a screenshot in PNG format.

### `GenerateJPEGUrl(options)` 
Gets a render link for a screenshot in JPEG format.

### `GeneratePDFUrl(options)` 
Gets a render link for a screenshot in PDF format.

# Popular Use Cases

## Taking a Full Page Screenshot

Want to take a screenshot of the full page from top to bottom?

For almost all formats, this is available by simply running a request with the full page option

```CS
UrlboxOptions options = Urlbox.Options(url: "https://google.com")
  .FullPage()
  .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```

This will generate you a tall render, from the top to the bottom of the page.

For video renders, there a bit more to it. To simply take a video of the website scrolling from top to bottom run a request like this:

```CS
UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com")
        .Format(Format.Mp4)
        .FullPage()
        .VideoScroll()
        .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```
This will render you a full page MP4 as the example below shows:

### [Example MP4 (Full Page)](../Examples/mp4.mp4)

## Taking a Mobile view screenshot

You may want to take a screenshot of a website/HTML as though it were being accessed from a mobile device.

To achieve this you can simply change the width of the viewport to suit your needs. Here's an example for mobile:

```CS
UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com")
  .Width(375)
  .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```

Which should render you something like the below example:

![](/Examples/mobile.png)

## Failing a request on 4XX-5XX

By default, Urlbox treats HTTP responses with status codes in the 400-599 range as successful renders, counting them toward your total render count.

This feature enables you to capture screenshots of error responses when needed. If you prefer your render requests to fail when the response falls within this range, you can configure this behavior by passing `FailOn4xx()` and/or `FailOn5xx` as such:

```CS
UrlboxOptions options = Urlbox.Options(url: "https://google.com")
  .FailOn4xx()
  .FailOn5xx()
  .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```

This can save you renders over the month, particularly when tackling websites like tricky social media pages.

If there is a failure, it will give you back a [UrlboxException](#urlboxexception). 

## Extracting Markdown/Metadata/HTML

In addition to your main render format for your URL/HTML, you can additionally render and save the same screenshot as HTML, Markdown and/or Metadata in the same request.

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(
                    url: "https://urlbox.com"
                )
                .Format(Format.Pdf)
                .SaveMarkdown() // This saves the same URL/HTML's content as a markdown file
                .SaveHtml() // This saves the same URL/HTML's content as its HTML
                .SaveMetadata() // This extracts the metadata, saves it and sends it back in the response.
                .Metadata() // This extracts the metadata from the URL/HTML, and sends it back in the response without saving it to the cloud.
                .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```

The JSON response is deserialized and turned into the SyncUrlboxResponse. The JSON response would look like this:

```JSON
{
  "renderUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.pdf",
  "size": 1048576,
  "htmlUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.html",
  "metadataUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.json",
  "markdownUrl": "https://renders.urlbox.com/ub-temp-renders/renders/662facc1f3b58e0a6df7a98b/2024/10/23/1b4df8c9-f347-4661-9b6a-1c969beb7522.md",
  "metadata": {
    "title": "Example Page",
    "description": "This is an example of metadata information.",
    "screenshot_date": "2024-11-06T12:34:56Z",
    "file_size": 1048576,
    "mime_type": "application/pdf"
  }
}
```

When using the screenshot and file generation methods from our SDK like `TakeScreenshot()`, `Render()` or `RenderAsync()`, responses will all be turned into a readable class instance for you, being either the `SyncUrlboxResponse` or `AsyncUrlboxResponse` for 200's.

When downloading metadata, you can opt to either save the metadata, or just return it in the JSON response as above. Our helper method `TakeScreenshotWithMetadata()` will not store the metadata so not produce a URL. It will instead only return the metadata object as above.

## Generating a Screenshot Using a Selector

There are times when you don't want to screenshot the entirety of a website. You may want to avoid manual cropping after taking your screenshot. You can take a screenshot of only the elements that you wish to using the selector.

Here's an example of using the selector option with our `Render(options)` method:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(url: "https://github.com")
    .Selector(".octicon-mark-github")
    .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```

This will take the ID selector ".octicon-mark-github", and return a screenshot that looks like this:

![](../Images/gh.png)

## Uploading to the cloud via an S3 bucket

For a typical render, we do the storing for you. You can opt to save the final screenshot to your own cloud provider.

We would _**highly**_ recommend you follow our S3 setup instructions. Setting up a cloud bucket can be tedious at the best of times, so [this](https://urlbox.com/docs/storage/configure-s3) part of our docs can help untangle the process.

In theory, we support any S3 compatible provider, though we have tested the following providers:

- BackBlaze B2
- AWS S3
- Cloudflare R2
- Google Cloud Storage
- Digital Ocean Spaces

If there's another cloud provider you would like to use, please try to reach out to us if you're struggling to get setup.

We allow for public CDN hosts, private buckets and buckets with object locking enabled.

Once you've set up your bucket, you can simply add `UrlboxOptions.UseS3()` to your options before making your request.

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(url: "https://google.com")
    .UseS3()
    .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```

## Using a Proxy

Proxies can really help get past issues like rendering social media sites, or sites that track your origin. We have a great piece in our [docs](https://urlbox.com/docs/guides/proxies) to get you started.

Simply pass in the proxy providers' details once you're set up, and we will make the request through that proxy. Here's an example:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");

UrlboxOptions options = Urlbox.Options(url: "https://google.com")
    .Proxy("http://brd-customer-hl_1a2b3c4d-zone-social_networks:ttpg162fe6e2@brd.superproxy.io:22225")
    .Build();

SyncUrlboxResponse response = await urlbox.Render(options);
```

## Using Webhooks

Webhooks are awesome. They save you time, money and headaches, and can quite equally cause just as many setting them up. Setting up a webhook with Urlbox has some optional steps, but we recommend you take them all for the most security.

Please look at our example directory in the repo.

### 1. Visit your Urlbox dashboard, and get your Webhook Secret.

Go to your [projects](https://urlbox.com/dashboard/projects) page, select a project (you may only have one if you're just starting out with Urlbox), and copy the webhook secret key.

### 2. Create your Urlbox instance in your C# project:

```CS
Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");
```

### 3. Make a request through any of our rendering methods. 

The most common use case for a webhook is when you need to use the `/async` endpoint to handle a larger render.

If you're developing locally, we would recommend using a service like [ngrok](https://ngrok.com/), and setting your webhook URL in the options to that ngrok endpoint. 

After you've added the endpoint, for example at the endpoint `/webhooks/urlbox`, make a request to that endpoint like this:

```CS
static async Task Main()
{
    Urlbox urlbox = Urlbox.FromCredentials("YOUR_KEY", "YOUR_SECRET", "YOUR_WEBHOOK_SECRET");
    
    UrlboxOptions options = Urlbox.Options(url: "https://google.com")
      .WebhookUrl("https://myapp.com/webhooks/urlbox")
      .Build();

    SyncUrlboxResponse response = await urlbox.Render(options);
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

There will also be our header `X-Urlbox-Signature` that should have a value like this: `t={timestamp},sha256={token}`.

Extract both the header and the content, and pass it into `Urlbox.VerifyWebhookSignature(header, content)`, which, if successful, will return you a [WebhookUrlboxResponse](#webhookurlboxresponse).

Please see the `Example` project in this repo which should help you get started.

---

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
  
- **`static UrlboxOptionsBuilder Options(string? url = null, string? html = null)`**  
  Creates a new instance of the Urlbox options builder. Requires a URL or HTML in the constructor to get started.

---

### Screenshot and File Generation Methods

- **`Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options);`**
- **`Task<AsyncUrlboxResponse> TakeScreenshot(UrlboxOptions options, int timeout);`**  
  Takes a screenshot asynchronously with a polling mechanism. Optional timeout to dictate when to stop polling.

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
- **`Task<SyncUrlboxResponse> Render(IDictionary<string, object> options);`**
  Sends a synchronous request to generate a render with the provided options, returning a direct response.

- **`Task<AsyncUrlboxResponse> RenderAsync(UrlboxOptions options);`**  
- **`Task<AsyncUrlboxResponse> RenderAsync(IDictionary<string, object> options);`**  
  Sends an asynchronous render request, providing a status URL for polling until completion.

---

### Download and File Handling Methods

- **`Task<string> DownloadAsBase64(UrlboxOptions options, string format = "png", bool sign = true);`**  
  Downloads a screenshot as a Base64-encoded string in the specified format. Optional format and whether to sign the render link.

- **`Task<string> DownloadAsBase64(string urlboxUrl);`**  
  Downloads the screenshot from the provided URL as a Base64-encoded string.

- **`Task<string> DownloadToFile(string urlboxUrl, string filename);`**  
  Downloads a screenshot from the URL and saves it to the specified file path.

- **`Task<string> DownloadToFile(UrlboxOptions options, string filename, string format = "png", bool sign = true);`**  
  Generates a screenshot based on options, then downloads and saves it as a file. Optional format and whether to sign the render link

---

### URL Generation Methods

- **`string GeneratePNGUrl(UrlboxOptions options, bool sign = true);`**  
  Generates a PNG URL based on the specified screenshot options.

- **`string GenerateJPEGUrl(UrlboxOptions options, bool sign = true);`**  
  Creates a JPEG URL using the provided rendering options.

- **`string GeneratePDFUrl(UrlboxOptions options, bool sign = true);`**  
  Generates a PDF URL for the specified screenshot options.

- **`string GenerateRenderLink(UrlboxOptions options, string format = "png", bool sign = true);`**  
  Constructs an Urlbox URL for the specified format and options.
 
- **`string GenerateSignedRenderLink(UrlboxOptions options, string format = "png");`**  
  Constructs an Urlbox URL for the specified format and options signed with the consumer's secret token.

---

### Status and Validation Methods

- **`Task<AsyncUrlboxResponse> GetStatus(string renderId);`**  
  Retrieves the current status of an asynchronous render request.

- **`bool VerifyWebhookSignature(string header, string content);`**  
  Verifies that a webhook signature originates from Urlbox using the configured webhook secret.


### Response Classes

When using the SDK, our deserializers will take the JSON response from any POST to the API and turn them into one of the following:

#### `SyncUrlboxResponse`

Properties:

- **`RenderUrl`** - The URL to run a GET request to in order to access your final render.
- **`Size`** - The size of the render in bytes.
- **`HtmlUrl`** - The URL to run a GET request to in order to access your final render as HTML.
- **`MhtmlUrl`** - The URL to run a GET request to in order to access your final render as MHTML.
- **`MetadataUrl`** - The URL to run a GET request to in order to access your final render as Metadata (JSON).
- **`MarkdownUrl`** - The URL to run a GET request to in order to access your final render as Markdown.
- **`Metadata`** - The Metadata object describing the rendered website.

#### `AsyncUrlboxResponse`

Properties:

- **`Status`** - One of `waiting`, `active`,  `failed`,  `delayed`,  `succeeded`.
- **`RenderId`** - The unique ID of the render request.
- **`StatusUrl`** - The URL to run a GET request to in order to find out if the render completed.
- **`Size`** - The size of the render in bytes.
- **`RenderUrl`** - The URL to run a GET request to in order to access your final render.
- **`HtmlUrl`** - The URL to run a GET request to in order to access your final render as HTML.
- **`MhtmlUrl`** - The URL to run a GET request to in order to access your final render as MHTML.
- **`MetadataUrl`** - The URL to run a GET request to in order to access your final render as Metadata (JSON).
- **`MarkdownUrl`** - The URL to run a GET request to in order to access your final render as Markdown.
- **`Metadata`** - The Metadata object describing the rendered website.

#### `WebhookUrlboxResponse`

Properties:

- **`Event`** - The event that happened to the render EG "render.succeeded"
- **`RenderId`** - The unique ID of the render request.
- **`Error`** - The error from Urlbox, showing the code, message and any errors
- **`Result`** - An instance of the SyncUrlboxResponse
- **`Meta`** - Includes the start and end times for the render

#### `UrlboxException`

Properties:

- **`RequestId`** - The unique ID of the render request.
- **`Code`** - The error code for the request. See a list [here](https://urlbox.com/docs/api#error-codes).
- **`Errors`** - A more detailed list of errors that occurred in the request.

#### `UrlboxMetadata`

Properties:

- **`UrlRequested`** - The original URL requested for rendering.
- **`UrlResolved`** - The final resolved URL after any redirects.
- **`Url`** - The canonical URL of the rendered page.
- **`Author`** - The author of the content, if available.
- **`Date`** - The publication date of the content, if available.
- **`Description`** - The meta description of the page.
- **`Image`** - The primary image of the page, if available.
- **`Logo`** - The logo associated with the page or publisher.
- **`Publisher`** - The name of the publisher of the content.
- **`Title`** - The title of the page.
- **`OgTitle`** - The Open Graph title of the page.
- **`OgImages`** - A list of Open Graph images found on the page.
- **`OgDescription`** - The Open Graph description of the page.
- **`OgUrl`** - The Open Graph URL of the page.
- **`OgType`** - The Open Graph type of the page (e.g., article, website).
- **`OgSiteName`** - The Open Graph site name of the page.
- **`OgLocale`** - The locale specified by Open Graph metadata.
- **`Charset`** - The character encoding used by the page.
- **`TwitterCard`** - The Twitter card type for the page.
- **`TwitterSite`** - The Twitter site associated with the page.
- **`TwitterCreator`** - The Twitter creator associated with the page.

### Available Enums

There are a number of options which are one of a select few. We have made enums for these, which can be accessed directly from the UrlboxOptions namespace:

ColorProfile - one of `Colorspingamma24`, `Default`, `Dp3`, `Hdr10`, `Rec2020`, `Scrgblinear`, `Srgb`

EngineVersion - one of `Latest`, `Lts`, `Stable`

Format - one of `Avif`, `Html`, `Jpeg`, `Jpg`, `Md`, `Mhtml`, `Mp4`, `Pdf`, `Png`, `Svg`, `Webm`, `Webp`

FullPageMode - one of `Native`, `Stitch`

ImgFit - one of `Contain`, `Cover`, `Fill`, `Inside`, `Outside`

ImgPosition - one of `Attention`, `Bottom`, `Center`, `Centre`, `East`, `Entropy`, `Left`, `LeftBottom`, `LeftTop`, `North`, `Northeast`, `Northwest`, `Right`, `RightBottom`, `RightTop`, `South`, `Southeast`, `Southwest`, `Top`, `West`

Media - one of `Print`, `Screen`

PdfMargin - one of `Default`, `Minimum`, `None`

PdfOrientation - one of `Landscape`, `Portait`

PdfPageSize - one of  `A0`, `A1`, `A2`, `A3`, `A4`, `A5`, `A6`, `Ledger`, `Legal`, `Letter`, `PdfPageSizeA0`, `PdfPageSizeA1`, `PdfPageSizeA2`, `PdfPageSizeA3`, `PdfPageSizeA4`, `PdfPageSizeA5`, `PdfPageSizeA6`, `PdfPageSizeLedger`, `PdfPageSizeLegal`, `PdfPageSizeLetter`, `PdfPageSizeTabloid`, `Tabloid`

ResponseType - one of `Base64`, `Binary`, `Json`, `Jsondebug`, `None`

S3Storageclass - one of `DeepArchive`, `Glacier`, `IntelligentTiering`, `OnezoneIa`, `Outposts`, `ReducedRedundancy`, `S3StorageclassDeepArchive`, `S3StorageclassGlacier`, `S3StorageclassIntelligentTiering`, `S3StorageclassOnezoneIa`, `S3StorageclassOutposts`, `S3StorageclassReducedRedundancy`, `S3StorageclassStandard`, `S3StorageclassStandardIa`, `Standard`, `StandardIa`

VideoCodec - one of `H264`, `Vp8`, `Vp9`

VideoEase - one of `BackIn`, `BackInout`, `BackOut`, `BounceIn`, `BounceInout`, `BounceOut`, `CircularIn`, `CircularInout`, `CircularOut`, `CubicIn`, `CubicInout`, `CubicOut`, `ElasticIn`, `ElasticInout`, `ElasticOut`, `ExponentialIn`, `ExponentialInout`, `ExponentialOut`, `LinearNone`, `QuadraticIn`, `QuadraticInout`, `QuadraticOut`, `QuarticIn`, `QuarticInout`, `QuarticOut`, `QuinticIn`, `QuinticInout`, `QuinticOut`, `SinusoidalIn`, `SinusoidalInout`, `SinusoidalOut`

VideoMethod - one of `Extension`, `Psr`, `Screencast`

VideoPreset - one of `Fast`, `Faster`, `Medium`, `Slow`, `Slower`, `Superfast`, `Ultrafast`, `Veryfast`, `Veryslow`

WaitUntil - one of `Domloaded`, `Loaded`, `Mostrequestsfinished`, `Requestsfinished`

## Examples

### [Example HTML](../Examples/html.html)
### [Example PDF](../Examples/pdf.pdf)
### [Example PDF Highlighting](../Examples/highlight.pdf)
### [Example PNG injecting Javascript](../Examples/javascript.png)

## Feedback

We hope that the above has given you enough of an understanding to suit your use case.

If you are still struggling, spot a bug, or have any suggestions, feel free to contact us at: `support@urlbox.com` or use our chat function on [our website](https://urlbox.com/).

Get rendering!

## Changelog

- 2.0.0 - Major overhaul - **Non-backward compatible changes included.**
  - Introduced fluent options builder with input validation.
  - Introduced options as a typed class.
  - Introduced webhook validation logic.
  - Upgraded test suite.
  - Created interfaces for DI.
  - Introduced post sync and async methods.
  - Introduced helper methods for common use cases.
  - Overhauled readme including an API reference.
  - Introduced logic and classes for side renders (save_html etc).
  - Introduced classes for different response types from urlbox api.
  - Added overhauls for render/renderAsync which take IDictionary for future proofing.
  - Overhauls readme.

Methods in previous versions of this SDK that would accept a Dictionary now take a standardised `UrlboxOptions` type.

- 1.0.2 - Further Updates to readme.
 
- 1.0.1 - Update Readme to replace instances of .io with .com.

- 1.0.0 - First release!
