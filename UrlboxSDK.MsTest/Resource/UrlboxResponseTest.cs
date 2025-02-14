using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Metadata.Resource;
using UrlboxSDK.Response.Resource;

namespace UrlboxSDK.MsTest.Resource;

[TestClass]
public class SyncUrlboxResponseTests
{
    [TestMethod]
    public void SyncUrlboxResponse_SuccessGetters()
    {
        string renderUrl = "renderurl";
        int size = 123;
        SyncUrlboxResponse response = new(renderUrl, size);
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.AreEqual(renderUrl, response.RenderUrl);
        Assert.AreEqual(size, response.Size);
    }

    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithHtmlGetters()
    {
        SyncUrlboxResponse response = new("renderurl", 123, htmlUrl: "url.html");
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.HtmlUrl);
    }

    [TestMethod]
    public void SyncUrlboxResponse_HtmlBadExtension()
    {
        Assert.ThrowsException<ArgumentException>(() => new SyncUrlboxResponse("renderurl", 123, htmlUrl: "url.png"));
    }

    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithMhtml()
    {
        SyncUrlboxResponse response = new("renderurl", 123, mhtmlUrl: "url.mhtml");
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.MhtmlUrl);
    }

    [TestMethod]
    public void SyncUrlboxResponse_MhtmlBadExtension()
    {
        Assert.ThrowsException<ArgumentException>(() => new SyncUrlboxResponse("renderurl", 123, mhtmlUrl: "url.png"));
    }

    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithMarkdown()
    {
        SyncUrlboxResponse response = new("renderurl", 123, markdownUrl: "url.md");
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.MarkdownUrl);
    }

    [TestMethod]
    public void SyncUrlboxResponse_MarkdownBadExtension()
    {
        Assert.ThrowsException<ArgumentException>(() => new SyncUrlboxResponse("renderurl", 123, mhtmlUrl: "url.png"));
    }

    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithMetadataUrl()
    {
        SyncUrlboxResponse response = new("renderurl", 123, metadataUrl: "url.json");
        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.MetadataUrl);
    }

    [TestMethod]
    public void SyncUrlboxResponse_MetadataBadExtension()
    {
        Assert.ThrowsException<ArgumentException>(() => new SyncUrlboxResponse("renderurl", 123, mhtmlUrl: "url.png"));
    }

    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithMetadata()
    {
        OgImage ogImage = new(
            url: "url",
            type: "type",
            width: "123",
            height: "123"
        );
        OgImage[] ogImages = new OgImage[] { ogImage, ogImage };
        UrlboxMetadata urlboxMetadata = new(
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
        SyncUrlboxResponse response = new("renderurl", 123, metadata: urlboxMetadata);

        Assert.IsInstanceOfType(response, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(response.Metadata);
    }

    [TestMethod]
    public void SyncUrlboxResponse_SuccessWithAll()
    {
        OgImage ogImage = new(url: "url", type: "type", width: "123", height: "123");
        UrlboxMetadata urlboxMetadata = new(
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

        SyncUrlboxResponse response = new(
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
        AsyncUrlboxResponse response = new(renderId: "renderId", statusUrl: "statusUrl", status: "succeeded");
        Assert.IsInstanceOfType(response, typeof(AsyncUrlboxResponse));
        Assert.AreEqual("succeeded", response.Status);
        Assert.AreEqual("statusUrl", response.StatusUrl);
        Assert.AreEqual("renderId", response.RenderId);
    }
}
