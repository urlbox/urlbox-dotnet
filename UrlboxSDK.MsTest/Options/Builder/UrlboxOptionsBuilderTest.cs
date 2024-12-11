using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Options.Builder;
using UrlboxSDK.Options.Resource;

namespace UrlboxSDK.MsTest.Options.Builder;

[TestClass]
public class UrlboxOptionsBuilderTests
{

    [TestMethod]
    public void BasicOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Format(Format.Png)
            .Width(1280)
            .Height(720)
            .FullPage()
            .Selector("#main")
            .Build();

        Assert.AreEqual(Format.Png, options.Format);
        Assert.AreEqual(1280, options.Width);
        Assert.AreEqual(720, options.Height);
        Assert.IsTrue(options.FullPage.HasValue && options.FullPage.Value.Bool == true);
        Assert.AreEqual("#main", options.Selector);
    }

    [TestMethod]
    public void BlockingOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .BlockAds()
            .HideCookieBanners()
            .ClickAccept()
            .BlockUrls("https://ads.example.com", "https://trackers.example.com")
            .BlockImages()
            .BlockFonts()
            .BlockMedias()
            .BlockStyles()
            .BlockScripts()
            .BlockFrames()
            .BlockFetch()
            .BlockXhr()
            .BlockSockets()
            .Build();

        Assert.IsTrue(options.BlockAds.HasValue && options.BlockAds.Value.Bool == true);
        Assert.IsTrue(options.HideCookieBanners.HasValue && options.HideCookieBanners.Value.Bool == true);
        Assert.IsTrue(options.ClickAccept.HasValue && options.ClickAccept.Value.Bool == true);
        if (options.BlockUrls.HasValue)
        {
            CollectionAssert.AreEqual(new[] { "https://ads.example.com", "https://trackers.example.com" }, options.BlockUrls.Value.StringArray);
        }
        Assert.IsTrue(options.BlockImages.HasValue && options.BlockImages.Value.Bool == true);
        Assert.IsTrue(options.BlockFonts.HasValue && options.BlockFonts.Value.Bool == true);
        Assert.IsTrue(options.BlockMedias.HasValue && options.BlockMedias.Value.Bool == true);
        Assert.IsTrue(options.BlockStyles.HasValue && options.BlockStyles.Value.Bool == true);
        Assert.IsTrue(options.BlockScripts.HasValue && options.BlockScripts.Value.Bool == true);
        Assert.IsTrue(options.BlockFrames.HasValue && options.BlockFrames.Value.Bool == true);
        Assert.IsTrue(options.BlockFetch.HasValue && options.BlockFetch.Value.Bool == true);
        Assert.IsTrue(options.BlockXhr.HasValue && options.BlockXhr.Value.Bool == true);
        Assert.IsTrue(options.BlockSockets.HasValue && options.BlockSockets.Value.Bool == true);
    }

    [TestMethod]
    public void CustomizeOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Js("document.body.style.backgroundColor = 'lightblue';")
            .Css("body { font-size: 16px; }")
            .DarkMode()
            .ReducedMotion()
            .Retina()
            .Build();

        Assert.AreEqual("document.body.style.backgroundColor = 'lightblue';", options.Js);
        Assert.AreEqual("body { font-size: 16px; }", options.Css);
        Assert.IsTrue(options.DarkMode.HasValue && options.DarkMode.Value.Bool == true);
        Assert.IsTrue(options.ReducedMotion.HasValue && options.ReducedMotion.Value.Bool == true);
        Assert.IsTrue(options.Retina.HasValue && options.Retina.Value.Bool == true);

    }

    [TestMethod]
    public void ScreenshotOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .ThumbWidth(200)
            .ThumbHeight(150)
            .ImgFit(ImgFit.Cover)
            .ImgPosition(ImgPosition.Center)
            .ImgBg("#FFFFFF")
            .ImgPad("10")
            .Quality(90)
            .Transparent()
            .MaxHeight(2000)
            .Download("screenshot.png")
            .Build();

        Assert.AreEqual(200, options.ThumbWidth);
        Assert.AreEqual(150, options.ThumbHeight);
        Assert.AreEqual(ImgFit.Cover, options.ImgFit);
        Assert.AreEqual(ImgPosition.Center, options.ImgPosition);
        Assert.AreEqual("#FFFFFF", options.ImgBg);
        Assert.AreEqual("10", options.ImgPad);
        Assert.AreEqual(90, options.Quality);
        Assert.IsTrue(options.Transparent.HasValue && options.Transparent.Value.Bool == true);
        Assert.AreEqual(2000, options.MaxHeight);
        Assert.AreEqual("screenshot.png", options.Download);
    }

    [TestMethod]
    public void PdfOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Format(Format.Pdf)
            .PdfPageSize(PdfPageSize.A4)
            .PdfPageRange("1-2")
            .PdfPageWidth(210)
            .PdfPageHeight(297)
            .PdfMargin(PdfMargin.Default)
            .PdfMarginTop(10)
            .PdfMarginRight(10)
            .PdfMarginBottom(10)
            .PdfMarginLeft(10)
            .PdfAutoCrop()
            .PdfScale(1.0)
            .PdfOrientation(PdfOrientation.Portrait)
            .PdfBackground()
            .DisableLigatures()
            .Media(Media.Print)
            .PdfShowHeader()
            .PdfHeader("Header content")
            .PdfShowFooter()
            .PdfFooter("Footer content")
            .Build();

        Assert.AreEqual(Format.Pdf, options.Format);
        Assert.AreEqual(PdfPageSize.A4, options.PdfPageSize);
        Assert.AreEqual("1-2", options.PdfPageRange);
        Assert.AreEqual(210, options.PdfPageWidth);
        Assert.AreEqual(297, options.PdfPageHeight);
        Assert.AreEqual(PdfMargin.Default, options.PdfMargin);
        Assert.AreEqual(10, options.PdfMarginTop);
        Assert.AreEqual(10, options.PdfMarginRight);
        Assert.AreEqual(10, options.PdfMarginBottom);
        Assert.AreEqual(10, options.PdfMarginLeft);
        Assert.IsTrue(options.PdfAutoCrop.HasValue && options.PdfAutoCrop.Value.Bool == true);
        Assert.AreEqual(1.0, options.PdfScale);
        Assert.AreEqual(PdfOrientation.Portrait, options.PdfOrientation);
        Assert.IsTrue(options.PdfBackground.HasValue && options.PdfBackground.Value.Bool == true);
        Assert.IsTrue(options.DisableLigatures.HasValue && options.DisableLigatures.Value.Bool == true);
        Assert.AreEqual(Media.Print, options.Media);
        Assert.IsTrue(options.PdfShowHeader.HasValue && options.PdfShowHeader.Value.Bool == true);
        Assert.AreEqual("Header content", options.PdfHeader);
        Assert.IsTrue(options.PdfShowFooter.HasValue && options.PdfShowFooter.Value.Bool == true);
        Assert.AreEqual("Footer content", options.PdfFooter);
    }

    [TestMethod]
    public void CacheOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Force()
            .Unique("unique-id")
            .Ttl(3600)
            .Build();

        Assert.IsTrue(options.Force.HasValue && options.Force.Value.Bool == true);
        Assert.AreEqual("unique-id", options.Unique);
        Assert.AreEqual(3600, options.Ttl);
    }

    [TestMethod]
    public void RequestOptions_ShouldSetCorrectly()
    {
        string[] expectedHeaderValue = new[] { "value1", "value2" };
        var options = Urlbox.Options(url: "https://example.com")
            .Header(expectedHeaderValue)
            .Cookie("sessionid=abc123")
            .UserAgent("Mozilla/5.0")
            .Platform("Win32")
            .AcceptLang("en-US")
            .Authorization("Bearer token")
            .Tz("UTC")
            .EngineVersion(EngineVersion.Latest)
            .Build();

        Assert.IsInstanceOfType(options.Header, typeof(SingleToArraySplit), "Header should be a SingleToArraySplit.");
        CollectionAssert.AreEqual(expectedHeaderValue, options.Header.Value.StringArray);

        Assert.AreEqual("sessionid=abc123", options.Cookie);
        Assert.AreEqual("Mozilla/5.0", options.UserAgent);
        Assert.AreEqual("Win32", options.Platform);
        Assert.AreEqual("en-US", options.AcceptLang);
        Assert.AreEqual("Bearer token", options.Authorization);
        Assert.AreEqual("UTC", options.Tz);
        Assert.AreEqual(EngineVersion.Latest, options.EngineVersion);
    }

    [TestMethod]
    public void WaitOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Delay(1000)
            .Timeout(30000)
            .WaitUntil(WaitUntil.Domloaded)
            .WaitFor("#content")
            .WaitToLeave(".loading")
            .WaitTimeout(5000)
            .FailIfSelectorMissing()
            .FailIfSelectorPresent()
            .FailOn4xx()
            .FailOn5xx()
            .Build();

        Assert.AreEqual(1000, options.Delay);
        Assert.AreEqual(30000, options.Timeout);
        Assert.AreEqual(WaitUntil.Domloaded, options.WaitUntil);
        Assert.AreEqual("#content", options.WaitFor);
        Assert.AreEqual(".loading", options.WaitToLeave);
        Assert.AreEqual(5000, options.WaitTimeout);
        Assert.IsTrue(options.FailIfSelectorMissing.HasValue && options.FailIfSelectorMissing.Value.Bool == true);
        Assert.IsTrue(options.FailIfSelectorPresent.HasValue && options.FailIfSelectorPresent.Value.Bool == true);
        Assert.IsTrue(options.FailOn4Xx.HasValue && options.FailOn4Xx.Value.Bool == true);
        Assert.IsTrue(options.FailOn5Xx.HasValue && options.FailOn4Xx.Value.Bool == true);
    }

    [TestMethod]
    public void AllOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(
                    url: "https://urlbox.com"
                )
                .WebhookUrl("https://example.com/webhook")
                .Format(Format.Pdf)
                .Width(1024)
                .Height(768)
                .FullPage()
                .Selector("#content")
                .Clip("0,0,400,400")
                .Gpu()
                .ResponseType(ResponseType.Json)
                .BlockAds()
                .HideCookieBanners()
                .ClickAccept()
                .BlockUrls("https://ads.com", "https://trackers.com")
                .BlockImages()
                .BlockFonts()
                .BlockMedias()
                .BlockStyles()
                .BlockScripts()
                .BlockFrames()
                .BlockFetch()
                .BlockXhr()
                .BlockSockets()
                .HideSelector(".banner")
                .Js("alert('Hello');")
                .Css("body { background: red; }")
                .DarkMode()
                .ReducedMotion()
                .Retina()
                .ThumbWidth(150)
                .ThumbHeight(150)
                .ImgFit(ImgFit.Cover)
                .ImgPosition(ImgPosition.Center)
                .ImgBg("#FFFFFF")
                .ImgPad("10")
                .Quality(90)
                .Transparent()
                .MaxHeight(2000)
                .Download("download.png")
                .PdfPageSize(PdfPageSize.A4)
                .PdfPageRange("1-2")
                .PdfPageWidth(210)
                .PdfPageHeight(297)
                .PdfMargin(PdfMargin.Default)
                .PdfMarginTop(10)
                .PdfMarginRight(10)
                .PdfMarginBottom(10)
                .PdfMarginLeft(10)
                .PdfAutoCrop()
                .PdfScale(1.0)
                .PdfOrientation(PdfOrientation.Portrait)
                .PdfBackground()
                .DisableLigatures()
                .Media(Media.Screen)
                .PdfShowHeader()
                .PdfHeader("Header content")
                .PdfShowFooter()
                .PdfFooter("Footer content")
                .Readable()
                .Force()
                .Unique("unique-id")
                .Ttl(3600)
                .Proxy("http://proxyserver.com")
                .Header(new string[] { "Authorization: Bearer token" })
                .Cookie("sessionid=abc123")
                .UserAgent("Mozilla/5.0")
                .Platform("Win32")
                .AcceptLang("en-US")
                .Authorization("Bearer token")
                .Tz("UTC")
                .EngineVersion(EngineVersion.Latest)
                .Delay(1000)
                .Timeout(30000)
                .WaitUntil(WaitUntil.Domloaded)
                .WaitFor("#content")
                .WaitToLeave(".loading")
                .WaitTimeout(5000)
                .FailIfSelectorMissing()
                .FailIfSelectorPresent()
                .FailOn4xx()
                .FailOn5xx()
                .ScrollTo("#bottom")
                .Click("#button")
                .ClickAll(".buttons")
                .Hover(".hover-element")
                .BgColor("#FAFAFA")
                .DisableJs()
                .FullPageMode(FullPageMode.Stitch)
                .FullWidth()
                .AllowInfinite()
                .SkipScroll()
                .DetectFullHeight()
                .MaxSectionHeight(500)
                .ScrollIncrement(200)
                .ScrollDelay(100)
                .Highlight("#highlight")
                .Highlightfg("#FF0000")
                .Highlightbg("#FFFF00")
                .Latitude(37.7749)
                .Longitude(-122.4194)
                .Accuracy(10)
                .UseS3()
                .S3Path("/screenshots")
                .S3Bucket("my-s3-bucket")
                .S3Endpoint("https://s3.amazonaws.com")
                .S3Region("us-west-1")
                .CdnHost("https://cdn.example.com")
                .S3Storageclass(S3Storageclass.Standard)
                .SaveHtml()
                .SaveMhtml()
                .SaveMarkdown()
                .SaveMetadata()
                .Metadata()
                .Build();

        Assert.IsInstanceOfType(options, typeof(UrlboxOptions));
        Assert.AreEqual("https://urlbox.com", options.Url);
    }

    [TestMethod]
    public void ValidateFullPageOptions_throws()
    {
        // FullPageMode should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .FullPageMode(FullPageMode.Stitch)
            .Build();
        });

        // ScrollIncrement should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .ScrollIncrement(100)
            .Build();
        });

        // ScrollDelay should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .ScrollDelay(500)
            .Build();
        });

        // DetectFullHeight should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .DetectFullHeight()
            .Build();
        });

        // MaxSectionHeight should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .MaxSectionHeight(2000)
            .Build();
        });

        // FullWidth should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .FullWidth()
            .Build();
        });
    }

    [TestMethod]
    public void ValidateS3Options_throws()
    {
        // S3Path should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .S3Path("/path/to/object")
            .Build();
        });

        // S3Bucket should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .S3Bucket("my-s3-bucket")
            .Build();
        });

        // S3Endpoint should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .S3Endpoint("https://s3.amazonaws.com")
            .Build();
        });

        // S3Region should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .S3Region("us-west-2")
            .Build();
        });

        // CdnHost should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .CdnHost("https://cdn.myhost.com")
            .Build();
        });

        // S3StorageClass should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format(Format.Png)
            .S3Storageclass(S3Storageclass.Standard)
            .Build();
        });
    }

    [TestMethod]
    public void ValidatePdfOptions_throws()
    {
        UrlboxOptionsBuilder builder = new(url: "https://urlbox.com");

        // PdfPageSize should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfPageSize(PdfPageSize.A4)
            .Build();
        });

        // PdfPageRange should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfPageRange("1-3")
            .Build();
        });

        // PdfPageWidth should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfPageWidth(210)
            .Build();
        });

        // PdfPageHeight should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfPageHeight(297)
            .Build();
        });

        // PdfMargin should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfMargin(PdfMargin.Default)
            .Build();
        });

        // PdfMarginTop should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfMarginTop(5)
            .Build();
        });

        // PdfMarginRight should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfMarginRight(5)
            .Build();
        });

        // PdfMarginBottom should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfMarginBottom(5)
            .Build();
        });

        // PdfMarginLeft should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfMarginLeft(5)
            .Build();
        });

        // PdfAutoCrop should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfAutoCrop()
            .Build();
        });

        // PdfScale should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfScale(1.5)
            .Build();
        });

        // PdfOrientation should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfOrientation(PdfOrientation.Portrait)
            .Build();
        });

        // PdfBackground should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfBackground()
            .Build();
        });

        // DisableLigatures should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").DisableLigatures()
            .Build();
        });

        // Media should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Media(Media.Print)
            .Build();
        });

        // Readable should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Readable()
            .Build();
        });

        // PdfShowHeader should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfShowHeader()
            .Build();
        });

        // PdfHeader should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfHeader("Header Content")
            .Build();
        });

        // PdfShowFooter should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfShowFooter()
            .Build();
        });

        // PdfFooter should throw an exception if format is not "pdf"
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").PdfFooter("Footer Content")
            .Build();
        });
    }

    [TestMethod]
    public void ValidateScreenshotOptions_throws()
    {
        // No thumb width or height but includes img fit
        var noThumbButImgFit = Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").ImgFit(ImgFit.Cover)
            .Build();
        });

        Assert.AreEqual(
            "Invalid Configuration: Image Fit is included despite ThumbWidth nor ThumbHeight being set.",
            noThumbButImgFit.Message
        );

        var thumbAndPositionButNoFit = Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com")
                .ThumbHeight(5)
                .ImgPosition(ImgPosition.North)
                .Build();
        });

        Assert.AreEqual(
            "Invalid Configuration: Image Position is included despite Image Fit not being set.",
            thumbAndPositionButNoFit.Message
        );
    }

    [TestMethod]
    public void ValidateScreenshotOptions_succeeds()
    {
        var heightAndImgFit =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbHeight(5)
            .ImgFit(ImgFit.Cover)
            .Build();

        var widthAndImgFit =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbWidth(5)
            .ImgFit(ImgFit.Cover)
            .Build();

        var justThumbHeight =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbHeight(5)
            .Build();

        var justThumbWidth =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbWidth(5)
            .Build();

        var heightAndImgFitCoverAndPosition =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbHeight(5)
            .ImgFit(ImgFit.Cover)
            .ImgPosition(ImgPosition.North)
            .Build();

        var heightAndImgFitContainAndPosition =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbHeight(5)
            .ImgFit(ImgFit.Contain)
            .Build();

        var widthAndImgFitCoverAndPosition =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbWidth(5)
            .ImgFit(ImgFit.Cover)
            .ImgPosition(ImgPosition.North)
            .Build();

        var widthAndImgFitContainAndPosition =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbWidth(5)
            .ImgFit(ImgFit.Contain)
            .Build();

        Assert.IsInstanceOfType(justThumbHeight, typeof(UrlboxOptions));
        Assert.IsInstanceOfType(justThumbWidth, typeof(UrlboxOptions));
        Assert.IsInstanceOfType(heightAndImgFit, typeof(UrlboxOptions));
        Assert.IsInstanceOfType(heightAndImgFitCoverAndPosition, typeof(UrlboxOptions));
        Assert.IsInstanceOfType(heightAndImgFitContainAndPosition, typeof(UrlboxOptions));
        Assert.IsInstanceOfType(widthAndImgFitCoverAndPosition, typeof(UrlboxOptions));
        Assert.IsInstanceOfType(widthAndImgFitContainAndPosition, typeof(UrlboxOptions));
        Assert.IsInstanceOfType(widthAndImgFit, typeof(UrlboxOptions));
    }

    [TestMethod]
    public void UrlboxOptionsBuilder_Resets()
    {
        var options = Urlbox.Options(url: "https://urlbox.com")
            .FullPage()
            .Build();

        var otherOptions = Urlbox.Options(url: "https://someotherurl.com").Build();

        Assert.IsFalse(otherOptions.FullPage.HasValue);
        Assert.AreNotSame(options, otherOptions);
    }
}
