
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK;

[TestClass]
public class UrlboxOptionsBuilderTests
{

    [TestMethod]
    public void BasicOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Format("png")
            .Width(1280)
            .Height(720)
            .FullPage()
            .Selector("#main")
            .Build();

        Assert.AreEqual("png", options.Format);
        Assert.AreEqual(1280, options.Width);
        Assert.AreEqual(720, options.Height);
        Assert.IsTrue(options.FullPage);
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

        Assert.IsTrue(options.BlockAds);
        Assert.IsTrue(options.HideCookieBanners);
        Assert.IsTrue(options.ClickAccept);
        CollectionAssert.AreEqual(new[] { "https://ads.example.com", "https://trackers.example.com" }, options.BlockUrls);
        Assert.IsTrue(options.BlockImages);
        Assert.IsTrue(options.BlockFonts);
        Assert.IsTrue(options.BlockMedias);
        Assert.IsTrue(options.BlockStyles);
        Assert.IsTrue(options.BlockScripts);
        Assert.IsTrue(options.BlockFrames);
        Assert.IsTrue(options.BlockFetch);
        Assert.IsTrue(options.BlockXhr);
        Assert.IsTrue(options.BlockSockets);
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
        Assert.IsTrue(options.DarkMode);
        Assert.IsTrue(options.ReducedMotion);
        Assert.IsTrue(options.Retina);
    }

    [TestMethod]
    public void ScreenshotOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .ThumbWidth(200)
            .ThumbHeight(150)
            .ImgFit("cover")
            .ImgPosition("center")
            .ImgBg("#FFFFFF")
            .ImgPad("10")
            .Quality(90)
            .Transparent()
            .MaxHeight(2000)
            .Download("screenshot.png")
            .Build();

        Assert.AreEqual(200, options.ThumbWidth);
        Assert.AreEqual(150, options.ThumbHeight);
        Assert.AreEqual("cover", options.ImgFit);
        Assert.AreEqual("center", options.ImgPosition);
        Assert.AreEqual("#FFFFFF", options.ImgBg);
        Assert.AreEqual("10", options.ImgPad);
        Assert.AreEqual(90, options.Quality);
        Assert.IsTrue(options.Transparent);
        Assert.AreEqual(2000, options.MaxHeight);
        Assert.AreEqual("screenshot.png", options.Download);
    }

    [TestMethod]
    public void PdfOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Format("pdf")
            .PdfPageSize("A4")
            .PdfPageRange("1-2")
            .PdfPageWidth(210)
            .PdfPageHeight(297)
            .PdfMargin("default")
            .PdfMarginTop(10)
            .PdfMarginRight(10)
            .PdfMarginBottom(10)
            .PdfMarginLeft(10)
            .PdfAutoCrop()
            .PdfScale(1.0)
            .PdfOrientation("portrait")
            .PdfBackground()
            .DisableLigatures()
            .Media("print")
            .PdfShowHeader()
            .PdfHeader("Header content")
            .PdfShowFooter()
            .PdfFooter("Footer content")
            .Build();

        Assert.AreEqual("pdf", options.Format);
        Assert.AreEqual("A4", options.PdfPageSize);
        Assert.AreEqual("1-2", options.PdfPageRange);
        Assert.AreEqual(210, options.PdfPageWidth);
        Assert.AreEqual(297, options.PdfPageHeight);
        Assert.AreEqual("default", options.PdfMargin);
        Assert.AreEqual(10, options.PdfMarginTop);
        Assert.AreEqual(10, options.PdfMarginRight);
        Assert.AreEqual(10, options.PdfMarginBottom);
        Assert.AreEqual(10, options.PdfMarginLeft);
        Assert.IsTrue(options.PdfAutoCrop);
        Assert.AreEqual(1.0, options.PdfScale);
        Assert.AreEqual("portrait", options.PdfOrientation);
        Assert.IsTrue(options.PdfBackground);
        Assert.IsTrue(options.DisableLigatures);
        Assert.AreEqual("print", options.Media);
        Assert.IsTrue(options.PdfShowHeader);
        Assert.AreEqual("Header content", options.PdfHeader);
        Assert.IsTrue(options.PdfShowFooter);
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

        Assert.IsTrue(options.Force);
        Assert.AreEqual("unique-id", options.Unique);
        Assert.AreEqual(3600, options.Ttl);
    }

    [TestMethod]
    public void RequestOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Header(new string[] { "Authorization: Bearer token" })
            .Cookie("sessionid=abc123")
            .UserAgent("Mozilla/5.0")
            .Platform("Win32")
            .AcceptLang("en-US")
            .Authorization("Bearer token")
            .Tz("UTC")
            .EngineVersion("1.0.0")
            .Build();

        Assert.IsInstanceOfType(options.Header, typeof(string[]), "Header should be a string array.");
        CollectionAssert.AreEqual(new[] { "Authorization: Bearer token" }, (string[])options.Header);

        Assert.AreEqual("sessionid=abc123", options.Cookie);
        Assert.AreEqual("Mozilla/5.0", options.UserAgent);
        Assert.AreEqual("Win32", options.Platform);
        Assert.AreEqual("en-US", options.AcceptLang);
        Assert.AreEqual("Bearer token", options.Authorization);
        Assert.AreEqual("UTC", options.Tz);
        Assert.AreEqual("1.0.0", options.EngineVersion);
    }

    [TestMethod]
    public void WaitOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(url: "https://example.com")
            .Delay(1000)
            .Timeout(30000)
            .WaitUntil(UrlboxOptions.WaitUntilOption.domloaded)
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
        Assert.AreEqual(UrlboxOptions.WaitUntilOption.domloaded, options.WaitUntil);
        Assert.AreEqual("#content", options.WaitFor);
        Assert.AreEqual(".loading", options.WaitToLeave);
        Assert.AreEqual(5000, options.WaitTimeout);
        Assert.IsTrue(options.FailIfSelectorMissing);
        Assert.IsTrue(options.FailIfSelectorPresent);
        Assert.IsTrue(options.FailOn4xx);
        Assert.IsTrue(options.FailOn5xx);
    }


    [TestMethod]
    public void AllOptions_ShouldSetCorrectly()
    {
        var options = Urlbox.Options(
                    url: "https://urlbox.com"
                )
                .WebhookUrl("https://example.com/webhook")
                .Format("pdf")
                .Width(1024)
                .Height(768)
                .FullPage()
                .Selector("#content")
                .Clip("0,0,400,400")
                .Gpu()
                .ResponseType("json")
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
                .ImgFit("cover")
                .ImgPosition("center")
                .ImgBg("#FFFFFF")
                .ImgPad("10")
                .Quality(90)
                .Transparent()
                .MaxHeight(2000)
                .Download("download.png")
                .PdfPageSize("A4")
                .PdfPageRange("1-2")
                .PdfPageWidth(210)
                .PdfPageHeight(297)
                .PdfMargin("default")
                .PdfMarginTop(10)
                .PdfMarginRight(10)
                .PdfMarginBottom(10)
                .PdfMarginLeft(10)
                .PdfAutoCrop()
                .PdfScale(1.0)
                .PdfOrientation("portrait")
                .PdfBackground()
                .DisableLigatures()
                .Media("screen")
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
                .EngineVersion("1.0.0")
                .Delay(1000)
                .Timeout(30000)
                .WaitUntil(UrlboxOptions.WaitUntilOption.domloaded)
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
                .FullPageMode(UrlboxOptions.FullPageModeOption.stitch)
                .FullWidth()
                .AllowInfinite()
                .SkipScroll()
                .DetectFullHeight()
                .MaxSectionHeight(500)
                .ScrollIncrement(200)
                .ScrollDelay(100)
                .Highlight("#highlight")
                .HighlightFg("#FF0000")
                .HighlightBg("#FFFF00")
                .Latitude(37.7749)
                .Longitude(-122.4194)
                .Accuracy(10)
                .UseS3()
                .S3Path("/screenshots")
                .S3Bucket("my-s3-bucket")
                .S3Endpoint("https://s3.amazonaws.com")
                .S3Region("us-west-1")
                .CdnHost("https://cdn.example.com")
                .S3StorageClass("STANDARD")
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
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .FullPageMode(UrlboxOptions.FullPageModeOption.stitch)
            .Build();
        });

        // ScrollIncrement should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .ScrollIncrement(100)
            .Build();
        });

        // ScrollDelay should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .ScrollDelay(500)
            .Build();
        });

        // DetectFullHeight should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .DetectFullHeight()
            .Build();
        });

        // MaxSectionHeight should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .MaxSectionHeight(2000)
            .Build();
        });

        // FullWidth should throw an exception
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
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
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .S3Path("/path/to/object")
            .Build();
        });

        // S3Bucket should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .S3Bucket("my-s3-bucket")
            .Build();
        });

        // S3Endpoint should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .S3Endpoint("https://s3.amazonaws.com")
            .Build();
        });

        // S3Region should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .S3Region("us-west-2")
            .Build();
        });

        // CdnHost should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .CdnHost("https://cdn.myhost.com")
            .Build();
        });

        // S3StorageClass should throw an exception if use_s3 is not enabled
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com").Format("png")
            .S3StorageClass("STANDARD")
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
            Urlbox.Options(url: "https://urlbox.com").PdfPageSize("A4")
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
            Urlbox.Options(url: "https://urlbox.com").PdfMargin("10mm")
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
            Urlbox.Options(url: "https://urlbox.com").PdfOrientation("portrait")
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
            Urlbox.Options(url: "https://urlbox.com").Media("print")
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
            Urlbox.Options(url: "https://urlbox.com").ImgFit("cover")
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
                .ImgPosition("north")
                .Build();
        });

        Assert.AreEqual(
            "Invalid Configuration: Image Position is included despite Image Fit not being set.",
            thumbAndPositionButNoFit.Message
        );

        var thumbAndPositionButFitWrong = Assert.ThrowsException<ArgumentException>(() =>
        {
            Urlbox.Options(url: "https://urlbox.com")
                .ThumbHeight(5)
                .ImgFit("notcoverorcontain")
                .ImgPosition("north")
                .Build();
        });

        Assert.AreEqual(
            "Invalid Configuration: Image Position is included despite Image Fit not being set to 'cover' or 'contain'.",
            thumbAndPositionButFitWrong.Message
        );
    }

    [TestMethod]
    public void ValidateScreenshotOptions_succeeds()
    {
        var heightAndImgFit =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbHeight(5)
            .ImgFit("cover")
            .Build();

        var widthAndImgFit =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbWidth(5)
            .ImgFit("cover")
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
            .ImgFit("cover")
            .ImgPosition("north")
            .Build();

        var heightAndImgFitContainAndPosition =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbHeight(5)
            .ImgFit("contain")
            .Build();

        var widthAndImgFitCoverAndPosition =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbWidth(5)
            .ImgFit("cover")
            .ImgPosition("north")
            .Build();

        var widthAndImgFitContainAndPosition =
        Urlbox.Options(url: "https://urlbox.com")
            .ThumbWidth(5)
            .ImgFit("contain")
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
    public void ValidateEngineVersionOptions_throws()
    {
        Assert.ThrowsException<ArgumentException>(() => Urlbox.Options(url: "https://urlbox.com")
            .EngineVersion("stable")
            .Latitude(0.01)
            .Build()
        );


        Assert.ThrowsException<ArgumentException>(() => Urlbox.Options(url: "https://urlbox.com")
            .EngineVersion("stable")
            .Longitude(0.01)
            .Build()
        );
    }

    [TestMethod]
    public void UrlboxOptionsBuilder_Resets()
    {
        var options = Urlbox.Options(url: "https://urlbox.com")
            .FullPage()
            .Build();


        var otherOptions = Urlbox.Options(url: "https://someotherurl.com").Build();

        Assert.IsFalse(otherOptions.FullPage);
        Assert.AreNotSame(options, otherOptions);
    }
}
