using UrlboxSDK.DI.Extension;
using UrlboxSDK;
using UrlboxSDK.Response.Resource;
using UrlboxSDK.Exception;
using UrlboxSDK.Options.Resource;
using UrlboxSDK.Webhook.Resource;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Urlbox to the service container
builder.Services.AddUrlbox(options =>
{
    // TODO Replace these with your keys from the dashboard: https://urlbox.com/dashboard/api 
    options.Key = "YOUR PUBLISHABLE API KEY HERE";
    options.Secret = "YOUR SECRET KEY HERE";
    // TODO optionally add this in to test out webhooks
    options.WebhookSecret = "YOUR WEBHOOK SECRET KEY HERE";
    // if you need to use one of our specific subdomains
    // options.BaseUrl = "https://api-eu.urlbox.com";
});

var app = builder.Build();

// Urlbox gets injected by the service container
app.MapGet("/", async (HttpContext context, IUrlbox urlbox) =>
{
    try
    {
        // Use the static .Options() method to choose your options
        UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com/docs")
            // Play around with various options here
            .Format(Format.Jpeg)
            // Want to test out webhooks? see the POST endpoint below
            // .WebhookUrl("https://YOUR NGROK FORWARDING ENDPOINT/webhook/urlbox")
            .Build();

        // Runs an async render, polls for success
        // AsyncUrlboxResponse takeScreenshotResponse = await urlbox.TakeScreenshot(options);

        // Runs an async render, gives status response
        AsyncUrlboxResponse renderAsyncResponse = await urlbox.RenderAsync(options);

        // Runs an sync render, waits for success before returning
        // SyncUrlboxResponse renderSyncResponse = await urlbox.Render(options);

        return Results.Json(new
        {
            message = "Screenshot generated!",
            // ResponseFromTakeScreenshot = takeScreenshotResponse,
            ResponseFromRenderAsync = renderAsyncResponse,
            // ResponseFromRenderSync = renderSyncResponse
        });
    }
    // Want to test how the exception looks? try this as the url in options: "https://notresolvableurlbox.com"
    catch (UrlboxException ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.RequestId);
        return Results.Json(new { message = "Failed to generate screenshot, urlbox exception", error = ex.Message, reqId = ex.RequestId });
    }
});

/* 
    Webhook Example:

    1. Make sure you've set your webhook secret in your Urlbox instantiation (line 16)
    2. Get ngrok (make an account and install on your computer) https://ngrok.com/
    3. Run this project with `dotnet run`.
    4. Using ngrok expose the localhost port the .net server runs on EG for 5096 `ngrok http 5096`
    4. Take the ngrok forwarding address shown in CLI EG https://2c85-80-41-190-113.ngrok-free.app and 
        replace the above .WebhookUrl() arg with it in your options, including the /webhook/urlbox endpoint.
    5. Make a request to the GET endpoint "/" above with one of the render methods, and Urlbox will make a POST to your ngrok endpoint /webhook/urlbox.
        EG: curl -i http://localhost:5096
*/
app.MapPost("/webhook/urlbox", async (HttpContext context, IUrlbox urlbox) =>
{
    using StreamReader stream = new StreamReader(context.Request.Body);

    if (!context.Request.Headers.TryGetValue("x-urlbox-signature", out var headerValue))
    {
        throw new Exception("Header 'x-urlbox-signature' not found.");
    }

    UrlboxWebhookResponse verifiedResponse = urlbox.VerifyWebhookSignature(headerValue.ToString(), await stream.ReadToEndAsync());

    string json = JsonSerializer.Serialize(verifiedResponse, new JsonSerializerOptions
    {
        WriteIndented = true
    });

    Console.WriteLine(json);
});

app.Run();
