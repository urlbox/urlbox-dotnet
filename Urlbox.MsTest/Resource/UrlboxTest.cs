using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using UrlboxSDK;
using System.Collections.Generic;
using UrlboxSDK.Exception;

[TestClass]
public class UrlTests
{
    UrlboxOptions urlboxAllOptions = new(url: "https://urlbox.com")
    {
        Width = 123,
        Height = 123,
        FullPage = true,
        Selector = "test",
        Clip = "test",
        Gpu = true,
        ResponseType = UrlboxOptions.ResponseTypeOption.json,
        BlockAds = true,
        HideCookieBanners = true,
        ClickAccept = true,
        BlockUrls = new string[] { "test", "test2" },
        BlockImages = true,
        BlockFonts = true,
        BlockMedias = true,
        BlockStyles = true,
        BlockScripts = true,
        BlockFrames = true,
        BlockFetch = true,
        BlockXhr = true,
        BlockSockets = true,
        HideSelector = "test",
        Js = "test",
        Css = "test",
        DarkMode = true,
        ReducedMotion = true,
        Retina = true,
        ThumbWidth = 123,
        ThumbHeight = 123,
        ImgFit = UrlboxOptions.ImgFitOption.contain,
        ImgPosition = UrlboxOptions.ImgPositionOption.northeast,
        ImgBg = "test",
        ImgPad = "12,10,10,10",
        Quality = 123,
        Transparent = true,
        MaxHeight = 123,
        Download = "test",
        PdfPageSize = UrlboxOptions.PdfPageSizeOption.Tabloid,
        PdfPageRange = "test",
        PdfPageWidth = 123,
        PdfPageHeight = 123,
        PdfMargin = UrlboxOptions.PdfMarginOption.@default,
        PdfMarginTop = 123,
        PdfMarginRight = 123,
        PdfMarginBottom = 123,
        PdfMarginLeft = 123,
        PdfAutoCrop = true,
        PdfScale = 0.12,
        PdfOrientation = UrlboxOptions.PdfOrientationOption.portrait,
        PdfBackground = true,
        DisableLigatures = true,
        Media = UrlboxOptions.MediaOption.print,
        PdfShowHeader = true,
        PdfHeader = "test",
        PdfShowFooter = true,
        PdfFooter = "test",
        Readable = true,
        Force = true,
        Unique = "test",
        Ttl = 123,
        Proxy = "test",
        Header = "test",
        Cookie = "test",
        UserAgent = "test",
        Platform = "Linux x86_64",
        AcceptLang = "test",
        Authorization = "test",
        Tz = "test",
        EngineVersion = "test",
        Delay = 123,
        Timeout = 123,
        WaitUntil = UrlboxOptions.WaitUntilOption.domloaded,
        WaitFor = "test",
        WaitToLeave = "test",
        WaitTimeout = 123,
        FailIfSelectorMissing = true,
        FailIfSelectorPresent = true,
        FailOn4xx = true,
        FailOn5xx = true,
        ScrollTo = "test",
        Click = "test",
        ClickAll = "test",
        Hover = "test",
        BgColor = "test",
        DisableJs = true,
        FullPageMode = UrlboxOptions.FullPageModeOption.stitch,
        FullWidth = true,
        AllowInfinite = true,
        SkipScroll = true,
        DetectFullHeight = true,
        MaxSectionHeight = 123,
        ScrollIncrement = 400,
        ScrollDelay = 123,
        Highlight = "test",
        HighlightFg = "test",
        HighlightBg = "test",
        Latitude = 0.12,
        Longitude = 0.12,
        Accuracy = 123,
        UseS3 = true,
        S3Path = "test",
        S3Bucket = "test",
        S3Endpoint = "test",
        S3Region = "test",
        CdnHost = "test",
        S3StorageClass = UrlboxOptions.S3StorageClassOptions.standard,
        WebhookUrl = "https://an-ngrok-endpoint"
    };

    private Urlbox urlbox;
    private Urlbox urlboxEu;
    private Urlbox dummyUrlbox;
    private RenderLinkGenerator renderLinkGenerator;

    [TestInitialize]
    public void TestInitialize()
    {
        // Build configuration to load user secrets
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<UrlTests>();

        IConfiguration configuration = builder.Build();

        // Attempt to load from environment variables first (for GH Actions)
        var urlboxKey = Environment.GetEnvironmentVariable("URLBOX_KEY")
                        ?? configuration["URLBOX_KEY"];  // Fallback to User Secrets for local dev

        var urlboxSecret = Environment.GetEnvironmentVariable("URLBOX_SECRET")
                           ?? configuration["URLBOX_SECRET"];  // Fallback to User Secrets for local dev

        if (string.IsNullOrEmpty(urlboxKey) || string.IsNullOrEmpty(urlboxSecret))
        {
            throw new ArgumentException("Please configure a URLBox key and secret.");
        }
        // With genuine API key and Secret
        urlbox = new Urlbox(urlboxKey, urlboxSecret, "webhook_secret");
        urlboxEu = new Urlbox(urlboxKey, urlboxSecret, "webhook_secret", "https://api-eu.urlbox.com");
        renderLinkGenerator = new RenderLinkGenerator("MY_API_KEY", "secret");

        // With dummy API key and Secret
        dummyUrlbox = new Urlbox("MY_API_KEY", "secret", "webhook_secret");
    }

    [TestMethod]
    public async Task GenerateRenderLink_WithAllOptions_Genuinely_Renders()
    {
        UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com").Build();
        var output = urlbox.GenerateRenderLink(options);

        var result = await urlbox.DownloadToFile(output, "test.png");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(String));
        Assert.IsTrue(result.Length >= 0);
    }

    [TestMethod]
    public async Task GenerateRenderLink_WithAllOptions_Signed_Genuinely_Renders()
    {
        UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com").Build();
        var output = urlbox.GenerateRenderLink(options, sign: true);

        var result = await urlbox.DownloadToFile(output, "testSigned.png");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(String));
        Assert.IsTrue(result.Length >= 0);
    }

    [TestMethod]
    public void GenerateRenderLink_WithAllOptions()
    {
        var output = dummyUrlbox.GenerateRenderLink(urlboxAllOptions);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?accept_lang=test&accuracy=123&allow_infinite=true&authorization=test&bg_color=test&block_ads=true&block_fetch=true&block_fonts=true&block_frames=true&block_images=true&block_medias=true&block_scripts=true&block_sockets=true&block_styles=true&block_urls=test%2Ctest2&block_xhr=true&cdn_host=test&click=test&click_accept=true&click_all=test&clip=test&cookie=test&css=test&dark_mode=true&delay=123&detect_full_height=true&disable_js=true&disable_ligatures=true&download=test&engine_version=test&fail_if_selector_missing=true&fail_if_selector_present=true&fail_on4xx=true&fail_on5xx=true&force=true&full_page=true&full_page_mode=stitch&full_width=true&gpu=true&header=test&height=123&hide_cookie_banners=true&hide_selector=test&highlight=test&highlight_bg=test&highlight_fg=test&hover=test&img_bg=test&img_fit=contain&img_pad=12%2C10%2C10%2C10&img_position=northeast&js=test&latitude=0.12&longitude=0.12&max_height=123&max_section_height=123&media=print&pdf_auto_crop=true&pdf_background=true&pdf_footer=test&pdf_header=test&pdf_margin=default&pdf_margin_bottom=123&pdf_margin_left=123&pdf_margin_right=123&pdf_margin_top=123&pdf_orientation=portrait&pdf_page_height=123&pdf_page_range=test&pdf_page_size=Tabloid&pdf_page_width=123&pdf_scale=0.12&pdf_show_footer=true&pdf_show_header=true&platform=Linux%20x86_64&proxy=test&quality=123&readable=true&reduced_motion=true&response_type=json&retina=true&s3_bucket=test&s3_endpoint=test&s3_path=test&s3_region=test&s3_storage_class=standard&scroll_delay=123&scroll_increment=400&scroll_to=test&selector=test&skip_scroll=true&thumb_height=123&thumb_width=123&timeout=123&transparent=true&ttl=123&tz=test&unique=test&url=https%3A%2F%2Furlbox.com&user_agent=test&use_s3=true&wait_for=test&wait_timeout=123&wait_to_leave=test&wait_until=domloaded&webhook_url=https%3A%2F%2Fan-ngrok-endpoint&width=123",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_eu()
    {
        var output = urlboxEu.GenerateRenderLink(urlboxAllOptions);

        Assert.AreEqual(
            "https://api-eu.urlbox.com/v1/rDksAC9TwlPFqvWw/png?accept_lang=test&accuracy=123&allow_infinite=true&authorization=test&bg_color=test&block_ads=true&block_fetch=true&block_fonts=true&block_frames=true&block_images=true&block_medias=true&block_scripts=true&block_sockets=true&block_styles=true&block_urls=test%2Ctest2&block_xhr=true&cdn_host=test&click=test&click_accept=true&click_all=test&clip=test&cookie=test&css=test&dark_mode=true&delay=123&detect_full_height=true&disable_js=true&disable_ligatures=true&download=test&engine_version=test&fail_if_selector_missing=true&fail_if_selector_present=true&fail_on4xx=true&fail_on5xx=true&force=true&full_page=true&full_page_mode=stitch&full_width=true&gpu=true&header=test&height=123&hide_cookie_banners=true&hide_selector=test&highlight=test&highlight_bg=test&highlight_fg=test&hover=test&img_bg=test&img_fit=contain&img_pad=12%2C10%2C10%2C10&img_position=northeast&js=test&latitude=0.12&longitude=0.12&max_height=123&max_section_height=123&media=print&pdf_auto_crop=true&pdf_background=true&pdf_footer=test&pdf_header=test&pdf_margin=default&pdf_margin_bottom=123&pdf_margin_left=123&pdf_margin_right=123&pdf_margin_top=123&pdf_orientation=portrait&pdf_page_height=123&pdf_page_range=test&pdf_page_size=Tabloid&pdf_page_width=123&pdf_scale=0.12&pdf_show_footer=true&pdf_show_header=true&platform=Linux%20x86_64&proxy=test&quality=123&readable=true&reduced_motion=true&response_type=json&retina=true&s3_bucket=test&s3_endpoint=test&s3_path=test&s3_region=test&s3_storage_class=standard&scroll_delay=123&scroll_increment=400&scroll_to=test&selector=test&skip_scroll=true&thumb_height=123&thumb_width=123&timeout=123&transparent=true&ttl=123&tz=test&unique=test&url=https%3A%2F%2Furlbox.com&user_agent=test&use_s3=true&wait_for=test&wait_timeout=123&wait_to_leave=test&wait_until=domloaded&webhook_url=https%3A%2F%2Fan-ngrok-endpoint&width=123",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_withMultipleCookies()
    {
        UrlboxOptions options = new(url: "https://urlbox.com");
        options.Cookie = new string[] {
            "some=cookie",
            "some=otherCookie",
            "some=thirdCookie"
        };
        var output = dummyUrlbox.GenerateRenderLink(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?cookie=some%3Dcookie%2Csome%3DotherCookie%2Csome%3DthirdCookie&url=https%3A%2F%2Furlbox.com",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_withOneCookie()
    {
        UrlboxOptions options = new(url: "https://urlbox.com");
        options.Cookie = "some=cookie";

        var output = dummyUrlbox.GenerateRenderLink(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?cookie=some%3Dcookie&url=https%3A%2F%2Furlbox.com",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_withMultipleBlockUrls()
    {
        UrlboxOptions options = new(url: "https://shopify.com");
        options.BlockUrls = new string[] { "cdn.shopify.com", "otherDomain" };

        var output = dummyUrlbox.GenerateRenderLink(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?block_urls=cdn.shopify.com%2CotherDomain&url=https%3A%2F%2Fshopify.com",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_withOneBlockUrl()
    {

        UrlboxOptions options = new(url: "https://shopify.com");
        options.BlockUrls = new string[] { "cdn.shopify.com" };

        var output = dummyUrlbox.GenerateRenderLink(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?block_urls=cdn.shopify.com&url=https%3A%2F%2Fshopify.com",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_WithUrlEncodedOptions()
    {
        var options = new UrlboxOptions(url: "urlbox.com");
        options.Width = 1280;
        options.ThumbWidth = 500;
        options.FullPage = true;
        options.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

        var output = dummyUrlbox.GenerateRenderLink(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?full_page=true&thumb_width=500&url=urlbox.com&user_agent=Mozilla%2F5.0%20%28Windows%20NT%206.1%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F41.0.2228.0%20Safari%2F537.36&width=1280",
                        output);
    }

    [TestMethod]
    public void GenerateRenderLink_UrlNeedsEncoding()
    {
        var options = new UrlboxOptions(url: "https://www.hatchtank.io/markup/index.html?url2png=true&board=demo_1645_1430");
        var output = dummyUrlbox.GenerateRenderLink(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?url=https%3A%2F%2Fwww.hatchtank.io%2Fmarkup%2Findex.html%3Furl2png%3Dtrue%26board%3Ddemo_1645_1430",
        output, "Not OK");
    }

    [TestMethod]
    public void GenerateRenderLink_WithUserAgent()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk");
        options.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";

        var output = dummyUrlbox.GenerateRenderLink(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?url=https%3A%2F%2Fbbc.co.uk&user_agent=Mozilla%2F5.0%20%28Macintosh%3B%20Intel%20Mac%20OS%20X%2010_12_6%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F62.0.3202.94%20Safari%2F537.36", output);
    }

    [TestMethod]
    public void GenerateRenderLink_IgnoreEmptyValuesAndFormat()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk");
        options.FullPage = false;
        options.ThumbWidth = 0;
        options.Delay = 0;
        options.Format = UrlboxOptions.FormatOption.pdf;
        options.Selector = "";
        options.WaitFor = "";
        options.BlockUrls = new string[] { };
        options.Cookie = "";

        var output = dummyUrlbox.GenerateRenderLink(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?url=https%3A%2F%2Fbbc.co.uk",
                        output);
    }

    [TestMethod]
    public void GenerateRenderLink_FormatWorks()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk");
        var output = dummyUrlbox.GenerateRenderLink(options, "jpeg");
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/jpeg?url=https%3A%2F%2Fbbc.co.uk", output, "Not OK!");
    }

    [TestMethod]
    public void GenerateRenderLink_WithHtml()
    {
        var options = new UrlboxOptions(html: "<h1>test</h1>");
        options.FullPage = true;
        var output = dummyUrlbox.GenerateRenderLink(options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?full_page=true&html=%3Ch1%3Etest%3C%2Fh1%3E", output);
    }

    [TestMethod]
    public void GenerateRenderLink_WithSimpleURL()
    {
        var options = new UrlboxOptions(url: "bbc.co.uk");
        var output = dummyUrlbox.GenerateRenderLink(options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?url=bbc.co.uk",
                        output, "Not OK");
    }

    [TestMethod]
    public void GenerateRenderLink_ShouldRemoveFormatFromQueryString()
    {
        var options = new UrlboxOptions(url: "https://urlbox.com")
        {
            Format = UrlboxOptions.FormatOption.png,
            FullPage = true
        };
        var output = renderLinkGenerator.GenerateRenderLink(Urlbox.BASE_URL, options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?full_page=true&url=https%3A%2F%2Furlbox.com", output);
    }

    [TestMethod]
    public async Task RenderSync_Succeeds()
    {
        UrlboxOptions options = new(url: "https://urlbox.com");
        options.ClickAccept = true;
        SyncUrlboxResponse result = await urlbox.Render(options);

        Assert.IsInstanceOfType(result, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task RenderSync_Dictionary_Succeeds()
    {
        IDictionary<string, object> options = new Dictionary<string, object>
        {
            { "click_accept", true },
            { "url", "https://urlbox.com" }
        };
        SyncUrlboxResponse result = await urlbox.Render(options);

        Assert.IsInstanceOfType(result, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task RenderSync_Succeeds_eu()
    {
        UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com").Build();
        options.ClickAccept = true;
        SyncUrlboxResponse result = await urlboxEu.Render(options);

        Assert.IsInstanceOfType(result, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task RenderSync_SucceedsWithAllSideRenders()
    {
        UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com")
        .ClickAccept()
        .SaveHtml()
        .Metadata()
        .SaveMetadata()
        .SaveMhtml()
        .SaveMarkdown()
        .Build();

        var result = await urlbox.Render(options);

        Assert.IsInstanceOfType(result, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(result.RenderUrl, "result.RenderUrl");
        Assert.IsNotNull(result.Size, "result.Size");
        Assert.IsNotNull(result.HtmlUrl, "result.HtmlUrl");
        Assert.IsNotNull(result.MhtmlUrl, "result.MhtmlUrl");
        Assert.IsNotNull(result.MarkdownUrl, "result.MarkdownUrl");
        Assert.IsNotNull(result.MetadataUrl, "result.MetadataUrl");
        Assert.IsNotNull(result.Metadata, "result.Metadata");
        Assert.IsNotNull(result.Metadata.Url, "result.Metadata.Url");
        Assert.IsNotNull(result.Metadata.UrlRequested, "result.Metadata.UrlRequested");
        Assert.IsNotNull(result.Metadata.UrlResolved, "result.Metadata.UrlResolved");
        Assert.IsNotNull(result.Metadata.OgImage, "result.Metadata.OgImage");
        Assert.IsNotNull(result.Metadata.OgImage[0].Height, "result.Metadata.OgImage[0].Height");
        Assert.IsNotNull(result.Metadata.OgImage[0].Url, "result.Metadata.OgImage[0].Url");
        Assert.IsNotNull(result.Metadata.OgImage[0].Width, "result.Metadata.OgImage[0].Width");
    }

    [TestMethod]
    public async Task RenderAsync_Succeeds()
    {
        UrlboxOptions options = new(url: "https://urlbox.com");
        options.ClickAccept = true;
        var result = await urlbox.RenderAsync(options);

        Assert.IsInstanceOfType(result, typeof(AsyncUrlboxResponse));
        Assert.IsNotNull(result.Status);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.StatusUrl);

        Assert.AreEqual("created", result.Status, "Render Async Failed");

        // Assert that optional fields should still be null
        Assert.IsNull(result.RenderUrl);
        Assert.IsNull(result.HtmlUrl);
        Assert.IsNull(result.MhtmlUrl);
        Assert.IsNull(result.MarkdownUrl);
        Assert.IsNull(result.MetadataUrl);
        Assert.IsNull(result.Metadata);
        Assert.IsNull(result.Size);
    }

    [TestMethod]
    public async Task RenderAsync_Dictionary_Succeeds()
    {
        IDictionary<string, object> options = new Dictionary<string, object>
        {
            { "click_accept", true },
            { "url", "https://urlbox.com" }
        };
        var result = await urlbox.RenderAsync(options);

        Assert.IsInstanceOfType(result, typeof(AsyncUrlboxResponse));
        Assert.IsNotNull(result.Status);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.StatusUrl);

        Assert.AreEqual("created", result.Status, "Render Async Failed");

        // Assert that optional fields should still be null
        Assert.IsNull(result.RenderUrl);
        Assert.IsNull(result.HtmlUrl);
        Assert.IsNull(result.MhtmlUrl);
        Assert.IsNull(result.MarkdownUrl);
        Assert.IsNull(result.MetadataUrl);
        Assert.IsNull(result.Metadata);
        Assert.IsNull(result.Size);
    }

    [TestMethod]
    public async Task Render_ThrowsException()
    {
        UrlboxOptions options = new(url: "https://FAKE_WEBSITE.com");
        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(async () => await urlbox.Render(options));

        Assert.IsTrue(exception.Message.Contains("Invalid options, please check errors -"));
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.IsNotNull(exception.Errors);
    }

    [TestMethod]
    public async Task Render_Dictionary_ThrowsException()
    {
        IDictionary<string, object> options = new Dictionary<string, object>
        {
            { "url", "https://FAKE_WEBSITE.com" }
        };
        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(async () => await urlbox.Render(options));

        Assert.IsTrue(exception.Message.Contains("Invalid options, please check errors -"));
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.IsNotNull(exception.Errors);
    }

    [TestMethod]
    public async Task Render_WithFailOn400_Throws()
    {
        UrlboxOptions options = Urlbox.Options(url: "test-site.urlbox.com/status/404").FailOn4xx().Build();

        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(async () => await urlbox.Render(options));
        Assert.AreEqual("Page returned 404 and fail_on_4xx was true", exception.Message);
        Assert.IsNull(exception.Code);
        Assert.IsNull(exception.Errors);
    }

    [TestMethod]
    public async Task Render_WithoutFailOn400_DoesntThrow()
    {
        UrlboxOptions options = Urlbox.Options(url: "https://example.com/someendpointthatgives404").Build();
        SyncUrlboxResponse result = await urlbox.Render(options);
        Assert.IsInstanceOfType(result, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task RenderAsync_ThrowsException()
    {
        UrlboxOptions options = new(url: "https://FAKE_WEBSITE.com");
        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(async () => await urlbox.RenderAsync(options));
        Assert.IsTrue(exception.Message.Contains("Invalid options, please check errors -"));
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.IsNotNull(exception.Errors);
    }

    [TestMethod]
    public async Task RenderAsync_Dictionary_ThrowsException()
    {
        IDictionary<string, object> options = new Dictionary<string, object>
        {
            { "url", "https://FAKE_WEBSITE.com" }
        };
        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(async () => await urlbox.RenderAsync(options));
        Assert.IsTrue(exception.Message.Contains("Invalid options, please check errors -"));
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.IsNotNull(exception.Errors);
    }

    [TestMethod]
    public void FromCredentials_Success()
    {
        var urlbox = Urlbox.FromCredentials("test_key", "test_secret", "test_webhook");
        Assert.IsInstanceOfType(urlbox, typeof(Urlbox));
    }

    [TestMethod]
    public void FromCredentials_Exception()
    {
        Assert.ThrowsException<ArgumentException>(() => Urlbox.FromCredentials("", "", ""));
    }

    [TestMethod]
    public async Task TakeScreenshot_Succeeds()
    {
        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Height = 125,
            Width = 125,
        };

        var result = await urlbox.TakeScreenshot(options);
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.Size);
    }


    [TestMethod]
    public async Task TakeScreenshot_SucceedsWithLargerTimeout()
    {
        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Height = 125,
            Width = 125,
        };

        var result = await urlbox.TakeScreenshot(options, 120000);
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task TakeScreenshot_TimeoutTooLarge()
    {
        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Height = 125,
            Width = 125,
        };

        var result = await Assert.ThrowsExceptionAsync<TimeoutException>(() => urlbox.TakeScreenshot(options, 1200001));
        Assert.AreEqual("Invalid Timeout Length. Must be between 5000 (5 seconds) and 120000 (2 minutes).", result.Message);
    }

    [TestMethod]
    public async Task TakeScreenshot_TimeoutTooSmall()
    {
        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Height = 125,
            Width = 125,
        };

        var result = await Assert.ThrowsExceptionAsync<TimeoutException>(() => urlbox.TakeScreenshot(options, 4999));
        Assert.AreEqual("Invalid Timeout Length. Must be between 5000 (5 seconds) and 120000 (2 minutes).", result.Message);
    }

    [TestMethod]
    public async Task TakePdf_Succeeds()
    {
        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Height = 125,
            Width = 125,
        };

        var result = await urlbox.TakePdf(options);
        Assert.IsNotNull(result.RenderUrl);
        StringAssert.Contains(result.RenderUrl, ".pdf", "The RenderUrl should contain '.pdf'.");
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task TakeMp4_Succeeds()
    {
        UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com")
        .Height(125)
        .Width(125)
        .Build();

        var result = await urlbox.TakeMp4(options);
        Assert.IsNotNull(result.RenderUrl);
        StringAssert.Contains(result.RenderUrl, ".mp4", "The RenderUrl should contain '.mp4'.");
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task TakeFullPage_Succeeds()
    {
        UrlboxOptions options = new(url: "https://urlbox.com");

        var result = await urlbox.TakeFullPageScreenshot(options);
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task TakeMobile_Succeeds()
    {
        UrlboxOptions options = new(url: "https://urlbox.com");

        var result = await urlbox.TakeMobileScreenshot(options);
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task TakeMetadata_Succeeds()
    {
        UrlboxOptions options = new(url: "https://urlbox.com");

        var result = await urlbox.TakeScreenshotWithMetadata(options);
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.Size);
        Assert.IsNotNull(result.Metadata);
    }
}

[TestClass]
public class DownloadTests
{
    private Urlbox urlbox;

    [TestInitialize]
    public void TestInitialize()
    {
        urlbox = new Urlbox("MY_API_KEY", "secret", "webhook_secret");
    }

    [TestMethod]
    public async Task TestDownloadToFile()
    {
        var urlboxUrl = "https://api.urlbox.com/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/5ee277f206869517d00cf1951f30d48ef9c64bfe/png?url=google.com";
        var result = await urlbox.DownloadToFile(urlboxUrl, "result.png");
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(String));
        Assert.IsTrue(result.Length >= 0);
    }

    [TestMethod]
    public async Task TestDownloadBase64()
    {
        var urlboxUrl = "https://api.urlbox.com/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/59148a4e454a2c7051488defdb8b246bdea61ace/jpeg?url=bbc.co.uk";
        var base64result = await urlbox.DownloadAsBase64(urlboxUrl);
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task TestDownloadFail()
    {
        var urlboxUrl = "https://api.urlbox.com/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/59148a4e454a2c7051488defdb8b246bdea61ac/jpeg?url=bbc.co.uk";
        var base64result = await Assert.ThrowsExceptionAsync<Exception>(() => urlbox.DownloadAsBase64(urlboxUrl));
        Assert.AreEqual(
            "Request failed: The generated token was incorrect. Please look in the docs (https://urlbox.io/docs) for how to generate your token correctly in the language you are using. TLDR: It should be the HMAC SHA256 of your query string, *signed* by your user secret, which you can find by logging into the urlbox dashboard",
            base64result.Message
        );
    }
}

[TestClass]
public class UrlboxWebhookValidatorTests
{
    private Urlbox urlbox;

    [TestInitialize]
    public void TestInitialize()
    {
        urlbox = new Urlbox("key", "secret", "webhook_secret");
    }

    [TestMethod]
    public void verifyWebhookSignature_Succeeds()
    {
        string urlboxSignature = "t=123456,sha256=41f85178517e8e031be5771ee4951bc3f6fbd871f41b4866546803576b1c3843";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        UrlboxWebhookResponse result = urlbox.VerifyWebhookSignature(urlboxSignature, content);

        Assert.AreEqual(result.Event, "render.succeeded");
        Assert.AreEqual(result.RenderId, "e9617143-2a95-4962-9cc9-d72f3c413b9c");

        Assert.AreEqual("https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png", result.Result.RenderUrl);
        Assert.AreEqual(359081, result.Result.Size);

        Assert.AreEqual(result.Meta.StartTime, "2024-01-11T23:32:11.908Z");
        Assert.AreEqual(result.Meta.EndTime, "2024-01-11T23:33:32.500Z");
    }

    [TestMethod]
    public void verifyWebhookSignature_FailsNoTimestamp()
    {
        string urlboxSignature = ",sha256=41f85178517e8e031be5771ee4951bc3f6fbd871f41b4866546803576b1c3843";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual(result.Message, "Unable to verify signature as header is empty or malformed. Please ensure you pass the `x-urlbox-signature` from the header of the webhook response.");
    }

    [TestMethod]
    public void verifyWebhookSignature_FailsNoSha()
    {
        string urlboxSignature = "t=123456,";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual(result.Message, "Unable to verify signature as header is empty or malformed. Please ensure you pass the `x-urlbox-signature` from the header of the webhook response.");
    }

    [TestMethod]
    public void Urlbox_createsWithWebhookValidator()
    {
        Urlbox urlbox = new("key", "secret", "webhook");
        // Shar of 'content' should not match 321, but method should run if 'webhook' passed.
        var result = Assert.ThrowsException<Exception>(() => urlbox.VerifyWebhookSignature("t=123,sha256=321", "content"));

        Assert.AreEqual(
            "Cannot verify that this response came from Urlbox. Double check that you're webhook secret is correct.",
            result.Message
        );
    }

    [TestMethod]
    public void Urlbox_throwsWhenWithoutWebhookValidator()
    {
        Urlbox urlbox = new("key", "secret");
        // Should throw bc no webhook set so no validator instance
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature("t=123,sha256=321", "content"));
        Assert.AreEqual(result.Message, "Please set your webhook secret in the Urlbox instance before calling this method.");
    }

    [TestMethod]
    public void verifyWebhookSignature_FailsShaEmpty()
    {
        string urlboxSignature = "t=123456,sha256=";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual("The signature could not be found, please ensure you are passing the x-urlbox-signature header.", result.Message);
    }

    [TestMethod]
    public void verifyWebhookSignature_FailsTimestampEmpty()
    {
        string urlboxSignature = "t=,sha256=41f85178517e8e031be5771ee4951bc3f6fbd871f41b4866546803576b1c3843";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual("The timestamp could not be found, please ensure you are passing the x-urlbox-signature header.", result.Message);
    }

    [TestMethod]
    public void verifyWebhookSignature_FailsNoComma()
    {
        string urlboxSignature = "t=12345sha256=41f85178517e8e031be5771ee4951bc3f6fbd871f41b4866546803576b1c3843";
        var content = "{\"event\":\"render.succeeded\",\"renderId\":\"e9617143-2a95-4962-9cc9-d72f3c413b9c\",\"result\":{\"renderUrl\":\"https://renders.urlbox.com/ub-temp-renders/renders/571f54138cd8b877077d3788/2024/1/11/e9617143-2a95-4962-9cc9-d72f3c413b9c.png\",\"size\":359081},\"meta\":{\"startTime\": \"2024-01-11T23:32:11.908Z\",\"endTime\":\"2024-01-11T23:33:32.500Z\"}}";
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature(urlboxSignature, content));
        Assert.AreEqual("Unable to verify signature as header is empty or malformed. Please ensure you pass the `x-urlbox-signature` from the header of the webhook response.", result.Message);
    }
}
