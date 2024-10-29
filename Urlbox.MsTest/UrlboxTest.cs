using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Screenshots;

[TestClass]
public class UrlTests
{
    UrlboxOptions urlboxAllOptions = new UrlboxOptions(url: "https://urlbox.com")
    {
        Width = 123,
        Height = 123,
        FullPage = true,
        Selector = "test",
        Clip = "test",
        Gpu = true,
        ResponseType = "test",
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
        ImgFit = "test",
        ImgPosition = "test",
        ImgBg = "test",
        ImgPad = "12,10,10,10",
        Quality = 123,
        Transparent = true,
        MaxHeight = 123,
        Download = "test",
        PdfPageSize = "test",
        PdfPageRange = "test",
        PdfPageWidth = 123,
        PdfPageHeight = 123,
        PdfMargin = "test",
        PdfMarginTop = 123,
        PdfMarginRight = 123,
        PdfMarginBottom = 123,
        PdfMarginLeft = 123,
        PdfAutoCrop = true,
        PdfScale = 0.12,
        PdfOrientation = "test",
        PdfBackground = true,
        DisableLigatures = true,
        Media = "test",
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
        Platform = "test",
        AcceptLang = "test",
        Authorization = "test",
        Tz = "test",
        EngineVersion = "test",
        Delay = 123,
        Timeout = 123,
        WaitUntil = "test",
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
        FullPageMode = "test",
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
        S3StorageClass = "test",
    };

    private Urlbox urlbox;
    private Urlbox dummyUrlbox;
    private UrlGenerator urlGenerator;

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
        urlGenerator = new UrlGenerator("MY_API_KEY", "secret");

        // With dummy API key and Secret
        dummyUrlbox = new Urlbox("MY_API_KEY", "secret", "webhook_secret");
    }

    [TestMethod]
    public void Urlbox_createsWithWebhookValidator()
    {
        Urlbox urlbox = new Urlbox("key", "secret", "webhook");
        // Shar of 'content' should not match 321, but method should run if 'webhook' passed.
        var result = urlbox.VerifyWebhookSignature("t=123,sha256=321", "content");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Urlbox_createsWithoutWebhookValidator()
    {
        Urlbox urlbox = new Urlbox("key", "secret");
        // Should throw bc no webhook set so no validator instance
        var result = Assert.ThrowsException<ArgumentException>(() => urlbox.VerifyWebhookSignature("t=123,sha256=321", "content"));
        Assert.AreEqual(result.Message, "Please set your webhook secret in the Urlbox instance before calling this method.");
    }

    [TestMethod]
    public void GenerateUrlboxUrl_WithAllOptions()
    {
        var output = dummyUrlbox.GenerateUrlboxUrl(urlboxAllOptions);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/6d4c63313408cb3127cb12811b4d7b1ab99579d3/png?url=https%3A%2F%2Furlbox.com&width=123&height=123&full_page=true&selector=test&clip=test&gpu=true&response_type=test&block_ads=true&hide_cookie_banners=true&click_accept=true&block_urls=test%2Ctest2&block_images=true&block_fonts=true&block_medias=true&block_styles=true&block_scripts=true&block_frames=true&block_fetch=true&block_xhr=true&block_sockets=true&hide_selector=test&js=test&css=test&dark_mode=true&reduced_motion=true&retina=true&thumb_width=123&thumb_height=123&img_fit=test&img_position=test&img_bg=test&img_pad=12%2C10%2C10%2C10&quality=123&transparent=true&max_height=123&download=test&pdf_page_size=test&pdf_page_range=test&pdf_page_width=123&pdf_page_height=123&pdf_margin=test&pdf_margin_top=123&pdf_margin_right=123&pdf_margin_bottom=123&pdf_margin_left=123&pdf_auto_crop=true&pdf_scale=0.12&pdf_orientation=test&pdf_background=true&disable_ligatures=true&media=test&pdf_show_header=true&pdf_header=test&pdf_show_footer=true&pdf_footer=test&readable=true&force=true&unique=test&ttl=123&proxy=test&header=test&cookie=test&user_agent=test&platform=test&accept_lang=test&authorization=test&tz=test&engine_version=test&delay=123&timeout=123&wait_until=test&wait_for=test&wait_to_leave=test&wait_timeout=123&fail_if_selector_missing=true&fail_if_selector_present=true&fail_on4xx=true&fail_on5xx=true&scroll_to=test&click=test&click_all=test&hover=test&bg_color=test&disable_js=true&full_page_mode=test&full_width=true&allow_infinite=true&skip_scroll=true&detect_full_height=true&max_section_height=123&scroll_increment=400&scroll_delay=123&highlight=test&highlight_fg=test&highlight_bg=test&latitude=0.12&longitude=0.12&accuracy=123&use_s3=true&s3_path=test&s3_bucket=test&s3_endpoint=test&s3_region=test&cdn_host=test&s3_storage_class=test",
            output
        );
    }

    [TestMethod]
    public void GenerateUrlboxUrl_withMultipleCookies()
    {
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com");
        options.Cookie = new string[] {
            "some=cookie",
            "some=otherCookie",
            "some=thirdCookie"
        };
        var output = dummyUrlbox.GenerateUrlboxUrl(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/875571589b83579c5bd98b2724fee1e942e6191b/png?url=https%3A%2F%2Furlbox.com&cookie=some%3Dcookie%2Csome%3DotherCookie%2Csome%3DthirdCookie",
            output
        );
    }

    [TestMethod]
    public void GenerateUrlboxUrl_withOneCookie()
    {
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com");
        options.Cookie = "some=cookie";

        var output = dummyUrlbox.GenerateUrlboxUrl(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/98443bad9c26ff2ec632a88a5ca81042a8f01f6e/png?url=https%3A%2F%2Furlbox.com&cookie=some%3Dcookie",
            output
        );
    }

    [TestMethod]
    public void GenerateUrlboxUrl_withMultipleBlockUrls()
    {
        UrlboxOptions options = new UrlboxOptions(url: "https://shopify.com");
        options.BlockUrls = new string[] { "cdn.shopify.com", "otherDomain" };

        var output = dummyUrlbox.GenerateUrlboxUrl(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/d8b2541a21203c7277ce450623e4d5821b09b7ce/png?url=https%3A%2F%2Fshopify.com&block_urls=cdn.shopify.com%2CotherDomain",
            output
        );
    }

    [TestMethod]
    public void GenerateUrlboxUrl_withOneBlockUrl()
    {

        UrlboxOptions options = new UrlboxOptions(url: "https://shopify.com");
        options.BlockUrls = new string[] { "cdn.shopify.com" };

        var output = dummyUrlbox.GenerateUrlboxUrl(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/1d35723ba9b60ec38ccdb01d066665c788002f17/png?url=https%3A%2F%2Fshopify.com&block_urls=cdn.shopify.com",
            output
        );
    }

    [TestMethod]
    public void GenerateUrlboxUrl_WithUrlEncodedOptions()
    {
        var options = new UrlboxOptions(url: "urlbox.com");
        options.Width = 1280;
        options.ThumbWidth = 500;
        options.FullPage = true;
        options.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

        var output = dummyUrlbox.GenerateUrlboxUrl(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/5727321d7976d07d9f24649e6db556b2a6a71d9d/png?url=urlbox.com&width=1280&full_page=true&thumb_width=500&user_agent=Mozilla%2F5.0%20%28Windows%20NT%206.1%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F41.0.2228.0%20Safari%2F537.36",
                        output);
    }

    [TestMethod]
    public void GenerateUrlboxUrl_UrlNeedsEncoding()
    {
        var options = new UrlboxOptions(url: "https://www.hatchtank.io/markup/index.html?url2png=true&board=demo_1645_1430");
        var output = dummyUrlbox.GenerateUrlboxUrl(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/4b8ac501f3aaccbea2081a7105302593174ebc23/png?url=https%3A%2F%2Fwww.hatchtank.io%2Fmarkup%2Findex.html%3Furl2png%3Dtrue%26board%3Ddemo_1645_1430",
        output, "Not OK");
    }

    [TestMethod]
    public void GenerateUrlboxUrl_WithUserAgent()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk");
        options.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";

        var output = dummyUrlbox.GenerateUrlboxUrl(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/c2708392a4d881b4816e61b3ed4d89ae4f2c4a57/png?url=https%3A%2F%2Fbbc.co.uk&user_agent=Mozilla%2F5.0%20%28Macintosh%3B%20Intel%20Mac%20OS%20X%2010_12_6%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F62.0.3202.94%20Safari%2F537.36", output);
    }

    [TestMethod]
    public void GenerateUrlboxUrl_IgnoreEmptyValuesAndFormat()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk");
        options.FullPage = false;
        options.ThumbWidth = 0;
        options.Delay = 0;
        options.Format = "pdf";
        options.Selector = "";
        options.WaitFor = "";
        options.BlockUrls = new string[] { };
        options.Cookie = "";

        var output = dummyUrlbox.GenerateUrlboxUrl(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/8e00ad9a8d7c4abcd462a9b8ec041c3661f13995/png?url=https%3A%2F%2Fbbc.co.uk",
                        output);
    }

    [TestMethod]
    public void GenerateUrlboxUrl_FormatWorks()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk");
        var output = dummyUrlbox.GenerateUrlboxUrl(options, "jpeg");
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/8e00ad9a8d7c4abcd462a9b8ec041c3661f13995/jpeg?url=https%3A%2F%2Fbbc.co.uk", output, "Not OK!");
    }

    [TestMethod]
    public void GenerateUrlboxUrl_WithHtml()
    {
        var options = new UrlboxOptions(html: "<h1>test</h1>");
        options.FullPage = true;
        var output = dummyUrlbox.GenerateUrlboxUrl(options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/6e911f299782a8de56b56f47d8670bd0f085f41b/png?html=%3Ch1%3Etest%3C%2Fh1%3E&full_page=true", output);
    }

    [TestMethod]
    public void GenerateUrlboxUrl_WithSimpleURL()
    {
        var options = new UrlboxOptions(url: "bbc.co.uk");
        var output = dummyUrlbox.GenerateUrlboxUrl(options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/75c9016e7f98f90f5eabfd348f3091f7bf625153/png?url=bbc.co.uk",
                        output, "Not OK");
    }

    [TestMethod]
    public void GenerateUrlboxUrl_ShouldRemoveFormatFromQueryString()
    {
        var options = new UrlboxOptions(url: "https://urlbox.com")
        {
            Format = "png",
            FullPage = true
        };
        var output = urlGenerator.GenerateUrlboxUrl(options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/bba10010e9ece486d34a82344170ae5b4dd5f347/png?url=https%3A%2F%2Furlbox.com&full_page=true", output);
    }

    [TestMethod]
    public async Task RenderSync_Succeeds()
    {
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com");
        options.ClickAccept = true;
        var result = await urlbox.Render(options);

        Assert.IsInstanceOfType(result, typeof(SyncUrlboxResponse));
        Assert.IsNotNull(result.RenderUrl);
        Assert.IsNotNull(result.Size);
    }

    [TestMethod]
    public async Task RenderSync_SucceedsWithAllSideRenders()
    {
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com");
        options.ClickAccept = true;
        options.SaveHtml = true;
        options.Metadata = true;
        options.SaveMetadata = true;
        options.SaveMhtml = true;
        options.SaveMarkdown = true;
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
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com");
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
    public async Task Render_ThrowsException()
    {
        UrlboxOptions options = new UrlboxOptions(url: "https://FAKE_WEBSITE.com");
        var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await urlbox.Render(options));
        Assert.IsTrue(exception.Message.Contains("Could not make post request to https://api.urlbox.com/v1/render/sync"));
    }

    [TestMethod]
    public async Task RenderAsync_ThrowsException()
    {
        UrlboxOptions options = new UrlboxOptions(url: "https://FAKE_WEBSITE.com");
        var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await urlbox.RenderAsync(options));
        Assert.IsTrue(exception.Message.Contains("Could not make post request to https://api.urlbox.com/v1/render/async"));
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
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com")
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
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com")
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
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com")
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
        UrlboxOptions options = new UrlboxOptions(url: "https://urlbox.com")
        {
            Height = 125,
            Width = 125,
        };

        var result = await Assert.ThrowsExceptionAsync<TimeoutException>(() => urlbox.TakeScreenshot(options, 4999));
        Assert.AreEqual("Invalid Timeout Length. Must be between 5000 (5 seconds) and 120000 (2 minutes).", result.Message);
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
        var result = urlbox.VerifyWebhookSignature(urlboxSignature, content);
        Assert.IsTrue(result);
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