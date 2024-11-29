using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK;

[TestClass]
public class SyncUrlboxResponseTests
{
    // Test SyncUrlboxResponse
    [TestMethod]
    public void SyncUrlboxResponse_SuccessGetters()
    {
        string renderUrl = "renderurl";
        int size = 123;
        SyncUrlboxResponse response = new SyncUrlboxResponse(renderUrl, size);
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.AreEqual(renderUrl, response.RenderUrl);
        Assert.AreEqual(size, response.Size);
    }

    // HtmlUrl
    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithHtmlGetters()
    {
        SyncUrlboxResponse response = new SyncUrlboxResponse("renderurl", 123, htmlUrl: "url.html");
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.HtmlUrl);
    }

    [TestMethod]
    public void SyncUrlboxResponse_HtmlBadExtension()
    {
        Assert.ThrowsException<ArgumentException>(() => new SyncUrlboxResponse("renderurl", 123, htmlUrl: "url.png"));
    }

    // MhtmlUrl
    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithMhtml()
    {
        SyncUrlboxResponse response = new SyncUrlboxResponse("renderurl", 123, mhtmlUrl: "url.mhtml");
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.MhtmlUrl);
    }

    [TestMethod]
    public void SyncUrlboxResponse_MhtmlBadExtension()
    {
        Assert.ThrowsException<ArgumentException>(() => new SyncUrlboxResponse("renderurl", 123, mhtmlUrl: "url.png"));
    }

    // MarkdownUrl
    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithMarkdown()
    {
        SyncUrlboxResponse response = new SyncUrlboxResponse("renderurl", 123, markdownUrl: "url.md");
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.MarkdownUrl);
    }

    [TestMethod]
    public void SyncUrlboxResponse_MarkdownBadExtension()
    {
        Assert.ThrowsException<ArgumentException>(() => new SyncUrlboxResponse("renderurl", 123, mhtmlUrl: "url.png"));
    }

    // MetadataUrl
    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithMetadataUrl()
    {
        SyncUrlboxResponse response = new SyncUrlboxResponse("renderurl", 123, metadataUrl: "url.json");
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.MetadataUrl);
    }

    [TestMethod]
    public void SyncUrlboxResponse_MetadataBadExtension()
    {
        Assert.ThrowsException<ArgumentException>(() => new SyncUrlboxResponse("renderurl", 123, mhtmlUrl: "url.png"));
    }

    // Metadata
    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithMetadata()
    {
        OgImage ogImage = new OgImage(
            url: "url",
            type: "type",
            width: "123",
            height: "123"
        );
        OgImage[] ogImages = new OgImage[] { ogImage, ogImage };
        UrlboxMetadata urlboxMetadata = new UrlboxMetadata(
            author: "author",
            date: "date",
            description: "description",
            image: "image",
            logo: "logo",
            publisher: "publisher",
            title: "title",
            url: "url",
            ogTitle: "ogTitle",
            ogImage: ogImages,
            ogLocale: "ogLocale",
            charset: "charset",
            urlRequested: "urlRequested",
            urlResolved: "urlResolved"
        );
        SyncUrlboxResponse response = new SyncUrlboxResponse("renderurl", 123, metadata: urlboxMetadata);

        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.Metadata);
    }

    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithAll()
    {
        OgImage ogImage = new OgImage(url: "url", type: "type", width: "123", height: "123");
        UrlboxMetadata urlboxMetadata = new UrlboxMetadata(
            author: "author",
            date: "date",
            description: "description",
            image: "image",
            logo: "logo",
            publisher: "publisher",
            title: "title",
            url: "url",
            ogTitle: "ogTitle",
            ogImage: new OgImage[] { ogImage, ogImage },
            ogLocale: "ogLocale",
            charset: "charset",
            urlRequested: "urlRequested",
            urlResolved: "urlResolved"
        );

        SyncUrlboxResponse response = new SyncUrlboxResponse(
            "renderurl",
             123,
             metadataUrl: "url.json",
             markdownUrl: "url.md",
             htmlUrl: "url.html",
             mhtmlUrl: "url.mhtml",
             metadata: urlboxMetadata
        );
        Assert.IsInstanceOfType(response,
            typeof(SyncUrlboxResponse)
        );

        Assert.IsNotNull(response.MetadataUrl);
        Assert.IsNotNull(response.MarkdownUrl);
        Assert.IsNotNull(response.HtmlUrl);
        Assert.IsNotNull(response.MhtmlUrl);
        Assert.IsNotNull(response.Size);
        Assert.IsNotNull(response.RenderUrl);
        Assert.IsNotNull(response.Metadata);
    }
}

[TestClass]
public class AsyncUrlboxResponseTests
{
    [TestMethod]
    public void AsyncUrlboxResponse_CreatesMinGetters()
    {
        AsyncUrlboxResponse response = new AsyncUrlboxResponse(renderId: "renderId", statusUrl: "statusUrl", status: "succeeded");
        Assert.IsInstanceOfType(response, typeof(AsyncUrlboxResponse));
        Assert.AreEqual("succeeded", response.Status);
        Assert.AreEqual("statusUrl", response.StatusUrl);
        Assert.AreEqual("renderId", response.RenderId);
    }
}


[TestClass]
public class UrlboxWebhookResponseTests
{
    [TestMethod]
    public void WebhookError_creates()
    {
        WebhookError error = new(message: "message");
        Assert.IsInstanceOfType(error, typeof(WebhookError));
        Assert.AreEqual("message", error.Message);
    }

    [TestMethod]
    public void WebhookMeta_creates()
    {
        Meta meta = new(startTime: "START", endTime: "END");
        Assert.IsInstanceOfType(meta, typeof(Meta));
        Assert.AreEqual("START", meta.StartTime);
        Assert.AreEqual("END", meta.EndTime);
    }

    [TestMethod]
    public void UrlboxWebhookResponse_CreatesMinGetters()
    {
        SyncUrlboxResponse response = new(
            renderUrl: "https://urlbox.com",
            size: 12345
        );

        Meta meta = new(startTime: "START", endTime: "END");

        UrlboxWebhookResponse webhookResponse = new(
            @event: "render.succeeded",
            renderId: "renderId",
            result: response,
            meta: meta
        );

        Assert.IsInstanceOfType(webhookResponse, typeof(UrlboxWebhookResponse));
        Assert.IsInstanceOfType(webhookResponse.Result, typeof(SyncUrlboxResponse));
        Assert.AreEqual("render.succeeded", webhookResponse.Event);
        Assert.AreSame(response, webhookResponse.Result);
        Assert.AreSame(meta, webhookResponse.Meta);
    }

    [TestMethod]
    public void UrlboxWebhookResponse_CreatesMinGettersWithError()
    {
        WebhookError error = new(message: "message");
        Meta meta = new(startTime: "START", endTime: "END");

        UrlboxWebhookResponse webhookResponse = new(
            @event: "render.succeeded",
            renderId: "renderId",
            error: error,
            meta: meta
        );

        Assert.IsInstanceOfType(webhookResponse, typeof(UrlboxWebhookResponse));
        Assert.AreEqual("render.succeeded", webhookResponse.Event);
        Assert.AreSame(error, webhookResponse.Error);
        Assert.AreSame(meta, webhookResponse.Meta);
    }
}
