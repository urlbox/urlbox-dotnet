using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Options.Resource;
using UrlboxSDK.Response.Resource;
using UrlboxSDK.Factory;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using UrlboxSDK.Exception;
using UrlboxSDK.MsTest.Utils;

namespace UrlboxSDK.MsTest.Resource;

[TestClass]
public class UrlTests
{
    readonly UrlboxOptions urlboxAllOptions = new(url: "https://urlbox.com")
    {
        Width = 123,
        Height = 123,
        FullPage = true,
        Selector = "test",
        Clip = "test",
        Gpu = true,
        ResponseType = ResponseType.Json,
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
        ImgFit = ImgFit.Contain,
        ImgPosition = ImgPosition.Northeast,
        ImgBg = "test",
        ImgPad = "12,10,10,10",
        Quality = 100,
        Transparent = true,
        MaxHeight = 123,
        Download = "test",
        PdfPageSize = PdfPageSize.Tabloid,
        PdfPageRange = "test",
        PdfPageWidth = 123,
        PdfPageHeight = 123,
        PdfMargin = PdfMargin.Default,
        PdfMarginTop = 123,
        PdfMarginRight = 123,
        PdfMarginBottom = 123,
        PdfMarginLeft = 123,
        PdfAutoCrop = true,
        PdfScale = 0.12,
        PdfOrientation = PdfOrientation.Portrait,
        PdfBackground = true,
        DisableLigatures = true,
        Media = Media.Print,
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
        EngineVersion = EngineVersion.Latest,
        Delay = 123,
        Timeout = 123,
        WaitUntil = WaitUntil.Domloaded,
        WaitFor = "test",
        WaitToLeave = "test",
        WaitTimeout = 123,
        FailIfSelectorMissing = true,
        FailIfSelectorPresent = true,
        FailOn4Xx = true,
        FailOn5Xx = true,
        ScrollTo = "test",
        Click = "test",
        ClickAll = "test",
        Hover = "test",
        BgColor = "test",
        DisableJs = true,
        FullPageMode = FullPageMode.Stitch,
        FullWidth = true,
        AllowInfinite = true,
        SkipScroll = true,
        DetectFullHeight = true,
        MaxSectionHeight = 123,
        ScrollIncrement = 400,
        ScrollDelay = 123,
        Highlight = "test",
        Highlightfg = "test",
        Highlightbg = "test",
        Latitude = 0.12,
        Longitude = 0.12,
        Accuracy = 123,
        UseS3 = true,
        S3Path = "test",
        S3Bucket = "test",
        S3Endpoint = "test",
        S3Region = "test",
        CdnHost = "test",
        S3Storageclass = S3Storageclass.Standard,
        WebhookUrl = "https://an-ngrok-endpoint"
    };
    private Urlbox urlbox;
    private RenderLinkFactory renderLinkFactory;
    private MockHttpClientFixture client;

    [TestInitialize]
    public void TestInitialize()
    {
        client = new MockHttpClientFixture();
        renderLinkFactory = new RenderLinkFactory("MY_API_KEY", "secret");
        urlbox = new(key: "MY_API_KEY", secret: "secret", webhookSecret: "webhook_secret", renderLinkFactory: renderLinkFactory, httpClient: client.HttpClient);
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
    public void GenerateRenderLink_WithAllOptions()
    {
        var output = urlbox.GenerateRenderLink(urlboxAllOptions);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?accept_lang=test&accuracy=123&allow_infinite=true&authorization=test&bg_color=test&block_ads=true&block_fetch=true&block_fonts=true&block_frames=true&block_images=true&block_medias=true&block_scripts=true&block_sockets=true&block_styles=true&block_urls=test%2Ctest2&block_xhr=true&cdn_host=test&click=test&click_accept=true&click_all=test&clip=test&cookie=test&css=test&dark_mode=true&delay=123&detect_full_height=true&disable_js=true&disable_ligatures=true&download=test&engine_version=latest&fail_if_selector_missing=true&fail_if_selector_present=true&fail_on_4xx=true&fail_on_5xx=true&force=true&full_page=true&full_page_mode=stitch&full_width=true&gpu=true&header=test&height=123&hide_cookie_banners=true&hide_selector=test&highlight=test&highlight_bg=test&highlight_fg=test&hover=test&img_bg=test&img_fit=contain&img_pad=12%2C10%2C10%2C10&img_position=northeast&js=test&latitude=0.12&longitude=0.12&max_height=123&max_section_height=123&media=print&pdf_auto_crop=true&pdf_background=true&pdf_footer=test&pdf_header=test&pdf_margin=default&pdf_margin_bottom=123&pdf_margin_left=123&pdf_margin_right=123&pdf_margin_top=123&pdf_orientation=portrait&pdf_page_height=123&pdf_page_range=test&pdf_page_size=tabloid&pdf_page_width=123&pdf_scale=0.12&pdf_show_footer=true&pdf_show_header=true&platform=Linux%20x86_64&proxy=test&quality=100&readable=true&reduced_motion=true&response_type=json&retina=true&s3_bucket=test&s3_endpoint=test&s3_path=test&s3_region=test&s3_storage_class=standard&scroll_delay=123&scroll_increment=400&scroll_to=test&selector=test&skip_scroll=true&thumb_height=123&thumb_width=123&timeout=123&transparent=true&ttl=123&tz=test&unique=test&url=https%3A%2F%2Furlbox.com&user_agent=test&use_s3=true&wait_for=test&wait_timeout=123&wait_to_leave=test&wait_until=domloaded&webhook_url=https%3A%2F%2Fan-ngrok-endpoint&width=123",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_TestFormatKey_withFailOnKeys()
    {
        var output = urlbox.GenerateRenderLink(
            Urlbox.Options(url: "testUrl").FailOn4xx().FailOn5xx().Build()
        );

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?fail_on_4xx=true&fail_on_5xx=true&url=testUrl",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_withMultipleCookies()
    {
        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Cookie = new string[] {
            "some=cookie",
            "some=otherCookie",
            "some=thirdCookie"
        }
        };
        var output = urlbox.GenerateRenderLink(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?cookie=some%3Dcookie%2Csome%3DotherCookie%2Csome%3DthirdCookie&url=https%3A%2F%2Furlbox.com",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_withOneCookie()
    {
        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Cookie = "some=cookie"
        };

        var output = urlbox.GenerateRenderLink(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?cookie=some%3Dcookie&url=https%3A%2F%2Furlbox.com",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_withMultipleBlockUrls()
    {
        UrlboxOptions options = new(url: "https://shopify.com")
        {
            BlockUrls = new string[] { "cdn.shopify.com", "otherDomain" }
        };

        var output = urlbox.GenerateRenderLink(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?block_urls=cdn.shopify.com%2CotherDomain&url=https%3A%2F%2Fshopify.com",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_withOneBlockUrl()
    {
        UrlboxOptions options = new(url: "https://shopify.com")
        {
            BlockUrls = new string[] { "cdn.shopify.com" }
        };

        var output = urlbox.GenerateRenderLink(options);

        Assert.AreEqual(
            "https://api.urlbox.com/v1/MY_API_KEY/png?block_urls=cdn.shopify.com&url=https%3A%2F%2Fshopify.com",
            output
        );
    }

    [TestMethod]
    public void GenerateRenderLink_WithUrlEncodedOptions()
    {
        var options = new UrlboxOptions(url: "urlbox.com")
        {
            Width = 1280,
            ThumbWidth = 500,
            FullPage = true,
            UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36"
        };

        var output = urlbox.GenerateRenderLink(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?full_page=true&thumb_width=500&url=urlbox.com&user_agent=Mozilla%2F5.0%20%28Windows%20NT%206.1%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F41.0.2228.0%20Safari%2F537.36&width=1280",
                        output);
    }

    [TestMethod]
    public void GenerateRenderLink_UrlNeedsEncoding()
    {
        var options = new UrlboxOptions(url: "https://www.hatchtank.io/markup/index.html?url2png=true&board=demo_1645_1430");
        var output = urlbox.GenerateRenderLink(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?url=https%3A%2F%2Fwww.hatchtank.io%2Fmarkup%2Findex.html%3Furl2png%3Dtrue%26board%3Ddemo_1645_1430",
        output, "Not OK");
    }

    [TestMethod]
    public void GenerateRenderLink_WithUserAgent()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk")
        {
            UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36"
        };

        var output = urlbox.GenerateRenderLink(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?url=https%3A%2F%2Fbbc.co.uk&user_agent=Mozilla%2F5.0%20%28Macintosh%3B%20Intel%20Mac%20OS%20X%2010_12_6%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F62.0.3202.94%20Safari%2F537.36", output);
    }

    [TestMethod]
    public void GenerateRenderLink_IgnoreEmptyValuesAndFormat()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk")
        {
            FullPage = false,
            ThumbWidth = 0,
            Delay = 0,
            Format = Format.Pdf,
            Selector = "",
            WaitFor = "",
            BlockUrls = new string[] { },
            Cookie = ""
        };

        var output = urlbox.GenerateRenderLink(options);
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?url=https%3A%2F%2Fbbc.co.uk",
                        output);
    }

    [TestMethod]
    public void GenerateRenderLink_FormatWorks()
    {
        var options = new UrlboxOptions(url: "https://bbc.co.uk");
        var output = urlbox.GenerateRenderLink(options, "jpeg");
        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/jpeg?url=https%3A%2F%2Fbbc.co.uk", output, "Not OK!");
    }

    [TestMethod]
    public void GenerateRenderLink_WithHtml()
    {
        var options = new UrlboxOptions(html: "<h1>test</h1>")
        {
            FullPage = true
        };
        var output = urlbox.GenerateRenderLink(options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?full_page=true&html=%3Ch1%3Etest%3C%2Fh1%3E", output);
    }

    [TestMethod]
    public void GenerateRenderLink_WithSimpleURL()
    {
        var options = new UrlboxOptions(url: "bbc.co.uk");
        var output = urlbox.GenerateRenderLink(options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?url=bbc.co.uk",
                        output, "Not OK");
    }

    [TestMethod]
    public void GenerateRenderLink_ShouldRemoveFormatFromQueryString()
    {
        var options = new UrlboxOptions(url: "https://urlbox.com")
        {
            Format = Format.Png,
            FullPage = true
        };
        var output = renderLinkFactory.GenerateRenderLink(Urlbox.BASE_URL, options);

        Assert.AreEqual("https://api.urlbox.com/v1/MY_API_KEY/png?full_page=true&url=https%3A%2F%2Furlbox.com", output);
    }

    [TestMethod]
    public async Task RenderSync_Succeeds()
    {
        string expectedResponse = @"
            {
                ""renderUrl"": ""https://example.com/screenshot.png"",
                ""size"": 123456,
                ""htmlUrl"": ""https://example.com/screenshot.html"",
                ""mhtmlUrl"": ""https://example.com/screenshot.mhtml"",
                ""metadataUrl"": ""https://example.com/metadata.json"",
                ""markdownUrl"": ""https://example.com/screenshot.md"",
                ""metadata"": {
                    ""urlRequested"": ""https://example.com"",
                    ""urlResolved"": ""https://example.com"",
                    ""url"": ""https://example.com""
                }
            }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/sync",
            (HttpStatusCode)200,
            expectedResponse
        );

        UrlboxOptions options = new(url: "https://urlbox.com") { ClickAccept = true };

        SyncUrlboxResponse result = await urlbox.Render(options);

        Assert.IsInstanceOfType(result, typeof(SyncUrlboxResponse));
        Assert.AreEqual(result.Size, 123456);
        Assert.AreEqual(result.RenderUrl, "https://example.com/screenshot.png");

        Assert.AreEqual(result.HtmlUrl, "https://example.com/screenshot.html");
        Assert.AreEqual(result.MhtmlUrl, "https://example.com/screenshot.mhtml");
        Assert.AreEqual(result.MetadataUrl, "https://example.com/metadata.json");
        Assert.AreEqual(result.MarkdownUrl, "https://example.com/screenshot.md");

        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual(result.Metadata.UrlRequested, "https://example.com");
        Assert.AreEqual(result.Metadata.UrlResolved, "https://example.com");
        Assert.AreEqual(result.Metadata.Url, "https://example.com");

    }

    [TestMethod]
    public async Task RenderSync_Dictionary_Succeeds()
    {
        string expectedResponse = @"
        {
            ""renderUrl"": ""https://example.com/screenshot.png"",
            ""size"": 123456,
            ""htmlUrl"": ""https://example.com/screenshot.html"",
            ""mhtmlUrl"": ""https://example.com/screenshot.mhtml"",
            ""metadataUrl"": ""https://example.com/metadata.json"",
            ""markdownUrl"": ""https://example.com/screenshot.md"",
            ""metadata"": {
                ""urlRequested"": ""https://example.com"",
                ""urlResolved"": ""https://example.com"",
                ""url"": ""https://example.com""
            }
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/sync",
            (HttpStatusCode)200,
            expectedResponse
        );


        IDictionary<string, object> options = new Dictionary<string, object>
            {
                { "click_accept", true },
                { "url", "https://urlbox.com" }
            };

        SyncUrlboxResponse result = await urlbox.Render(options);

        Assert.IsInstanceOfType(result, typeof(SyncUrlboxResponse));
        Assert.AreEqual(result.RenderUrl, "https://example.com/screenshot.png");
        Assert.AreEqual(result.Size, 123456);

        Assert.AreEqual(result.HtmlUrl, "https://example.com/screenshot.html");
        Assert.AreEqual(result.MhtmlUrl, "https://example.com/screenshot.mhtml");
        Assert.AreEqual(result.MetadataUrl, "https://example.com/metadata.json");
        Assert.AreEqual(result.MarkdownUrl, "https://example.com/screenshot.md");

        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual(result.Metadata.UrlRequested, "https://example.com");
        Assert.AreEqual(result.Metadata.UrlResolved, "https://example.com");
        Assert.AreEqual(result.Metadata.Url, "https://example.com");
    }

    [TestMethod]
    public async Task RenderAsync_Succeeds()
    {
        string expectedResponse = @"
        {
            ""status"": ""created"",
            ""renderId"": ""abc123"",
            ""statusUrl"": ""https://example.com/status""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/async",
            (HttpStatusCode)200,
            expectedResponse
        );


        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            ClickAccept = true
        };

        AsyncUrlboxResponse result = await urlbox.RenderAsync(options);

        Assert.IsInstanceOfType(result, typeof(AsyncUrlboxResponse));
        Assert.IsNotNull(result.Status);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.StatusUrl);

        Assert.AreEqual("created", result.Status, "Render Async Failed");
        Assert.AreEqual("abc123", result.RenderId);
        Assert.AreEqual("https://example.com/status", result.StatusUrl);

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
        string expectedResponse = @"
        {
            ""status"": ""created"",
            ""renderId"": ""abc123"",
            ""statusUrl"": ""https://example.com/status""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/async",
            (HttpStatusCode)200,
            expectedResponse
        );


        IDictionary<string, object> options = new Dictionary<string, object>
        {
            { "click_accept", true },
            { "url", "https://urlbox.com" }
        };

        AsyncUrlboxResponse result = await urlbox.RenderAsync(options);

        Assert.IsInstanceOfType(result, typeof(AsyncUrlboxResponse));
        Assert.IsNotNull(result.Status);
        Assert.IsNotNull(result.RenderId);
        Assert.IsNotNull(result.StatusUrl);

        Assert.AreEqual("created", result.Status, "Render Async Failed");
        Assert.AreEqual("abc123", result.RenderId);
        Assert.AreEqual("https://example.com/status", result.StatusUrl);

        // Should be null as not succeeded yet
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
        string errorResponse = @"
        {
            ""error"": {
                ""message"": ""Invalid options, please check errors - {\""url\"":[\""error resolving URL - ENOTFOUND fakesite.com\""]}"",
                ""code"": ""InvalidOptions"",
                ""errors"": ""{\""url\"":[\""error resolving URL - ENOTFOUND fakesite.com\""]}""
            },
            ""requestId"": ""7be80323-3b75-4cf1-960f-13e9f3ff404c""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/sync",
            (HttpStatusCode)400,
            errorResponse
        );


        UrlboxOptions options = new(url: "https://fakesite.com");

        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(
            async () => await urlbox.Render(options)
        );

        Assert.IsTrue(exception.Message.Contains("Invalid options, please check errors -"));
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.IsNotNull(exception.Errors);
        Assert.IsTrue(exception.Errors.Contains("error resolving URL - ENOTFOUND fakesite.com"));
    }

    [TestMethod]
    public async Task Render_Dictionary_ThrowsException()
    {
        string errorResponse = @"
        {
            ""error"": {
                ""message"": ""Invalid options, please check errors - {\""url\"":[\""error resolving URL - ENOTFOUND fakeSite.com\""]}"",
                ""code"": ""InvalidOptions"",
                ""errors"": ""{\""url\"":[\""error resolving URL - ENOTFOUND fakeSite.com\""]}""
            },
            ""requestId"": ""7be80323-3b75-4cf1-960f-13e9f3ff404c""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/sync",
            (HttpStatusCode)400,
            errorResponse
        );


        IDictionary<string, object> options = new Dictionary<string, object>
        {
            { "url", "https://fakesite.com" }
        };

        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(
            async () => await urlbox.Render(options)
        );

        Assert.IsTrue(exception.Message.Contains("Invalid options, please check errors -"));
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.IsNotNull(exception.Errors);
        Assert.IsTrue(exception.Errors.Contains("error resolving URL - ENOTFOUND fakeSite.com"));
    }

    [TestMethod]
    public async Task RenderAsync_ThrowsException()
    {
        string errorResponse = @"
        {
            ""error"": {
                ""message"": ""Invalid options, please check errors - {\""url\"":[\""error resolving URL - ENOTFOUND fakeSite.com\""]}"",
                ""code"": ""InvalidOptions"",
                ""errors"": ""{\""url\"":[\""error resolving URL - ENOTFOUND fakeSite.com\""]}""
            },
            ""requestId"": ""7be80323-3b75-4cf1-960f-13e9f3ff404c""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/async",
            (HttpStatusCode)400,
            errorResponse
        );


        UrlboxOptions options = new(url: "https://fakesite.com");

        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(
            async () => await urlbox.RenderAsync(options)
        );

        Assert.IsTrue(exception.Message.Contains("Invalid options, please check errors -"));
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.IsNotNull(exception.Errors);
        Assert.IsTrue(exception.Errors.Contains("error resolving URL - ENOTFOUND fakeSite.com"));
    }

    [TestMethod]
    public async Task RenderAsync_Dictionary_ThrowsException()
    {
        string errorResponse = @"
        {
            ""error"": {
                ""message"": ""Invalid options, please check errors - {\""url\"":[\""error resolving URL - ENOTFOUND fakesite.com\""]}"",
                ""code"": ""InvalidOptions"",
                ""errors"": ""{\""url\"":[\""error resolving URL - ENOTFOUND fakesite.com\""]}""
            },
            ""requestId"": ""7be80323-3b75-4cf1-960f-13e9f3ff404c""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/async",
            (HttpStatusCode)400,
            errorResponse
        );


        IDictionary<string, object> options = new Dictionary<string, object>
        {
            { "url", "https://fakesite.com" }
        };

        var exception = await Assert.ThrowsExceptionAsync<UrlboxException>(
            async () => await urlbox.RenderAsync(options)
        );

        Assert.IsTrue(exception.Message.Contains("Invalid options, please check errors -"));
        Assert.AreEqual("InvalidOptions", exception.Code);
        Assert.IsNotNull(exception.Errors);
        Assert.IsTrue(exception.Errors.Contains("error resolving URL - ENOTFOUND fakesite.com"));
    }


    [TestMethod]
    public async Task TakeScreenshot_Succeeds()
    {
        string initialResponse = @"
        {
            ""status"": ""created"",
            ""renderId"": ""abc123"",
            ""statusUrl"": ""https://example.com/status""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/async",
            (HttpStatusCode)200,
            initialResponse
        );

        string statusResponse = @"
        {
            ""status"": ""succeeded"",
            ""renderId"": ""abc123"",
            ""renderUrl"": ""https://example.com/screenshot.png"",
            ""size"": 123456
        }";

        client.StubRequest(
            HttpMethod.Get,
            $"{Urlbox.BASE_URL}/v1/render/abc123",
            (HttpStatusCode)200,
            statusResponse
        );


        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Height = 125,
            Width = 125
        };

        AsyncUrlboxResponse result = await urlbox.TakeScreenshot(options);

        Assert.IsNotNull(result);
        Assert.AreEqual("abc123", result.RenderId);
        Assert.AreEqual("https://example.com/screenshot.png", result.RenderUrl);
        Assert.AreEqual(123456, result.Size);
        Assert.AreEqual("succeeded", result.Status);
    }

    [TestMethod]
    public async Task TakeScreenshot_SucceedsWithLargerTimeout()
    {
        string initialResponse = @"
        {
            ""status"": ""created"",
            ""renderId"": ""abc123"",
            ""statusUrl"": ""https://example.com/status""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/async",
            (HttpStatusCode)200,
            initialResponse
        );

        string statusResponse = @"
        {
            ""status"": ""succeeded"",
            ""renderId"": ""abc123"",
            ""renderUrl"": ""https://example.com/screenshot.png"",
            ""size"": 123456
        }";

        client.StubRequest(
            HttpMethod.Get,
            $"{Urlbox.BASE_URL}/v1/render/abc123",
            (HttpStatusCode)200,
            statusResponse
        );


        UrlboxOptions options = new(url: "https://urlbox.com")
        {
            Height = 125,
            Width = 125
        };

        // Use a larger timeout value
        AsyncUrlboxResponse result = await urlbox.TakeScreenshot(options, 120000);

        Assert.IsNotNull(result);
        Assert.AreEqual("abc123", result.RenderId);
        Assert.AreEqual("https://example.com/screenshot.png", result.RenderUrl);
        Assert.AreEqual(123456, result.Size);
        Assert.AreEqual("succeeded", result.Status);
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
    public async Task TakeMetadata_Succeeds()
    {
        string initialResponse = @"
        {
            ""status"": ""created"",
            ""renderId"": ""abc123"",
            ""statusUrl"": ""https://example.com/status""
        }";

        client.StubRequest(
            HttpMethod.Post,
            Urlbox.BASE_URL + "/v1/render/async",
            (HttpStatusCode)200,
            initialResponse
        );

        string statusResponse = @"
        {
            ""status"": ""succeeded"",
            ""renderId"": ""abc123"",
            ""renderUrl"": ""https://example.com/screenshot.png"",
            ""size"": 123456,
            ""metadata"": {
                ""urlRequested"": ""https://urlbox.com"",
                ""url"": ""https://urlbox.com"",
                ""urlResolved"": ""https://example.com"",
                ""title"": ""Example Title""
            }
        }";

        client.StubRequest(
            HttpMethod.Get,
            $"{Urlbox.BASE_URL}/v1/render/abc123",
            (HttpStatusCode)200,
            statusResponse
        );


        UrlboxOptions options = new(url: "https://urlbox.com");

        AsyncUrlboxResponse result = await urlbox.TakeScreenshotWithMetadata(options);

        Assert.IsNotNull(result);
        Assert.AreEqual("abc123", result.RenderId);
        Assert.AreEqual("https://example.com/screenshot.png", result.RenderUrl);
        Assert.AreEqual(123456, result.Size);
        Assert.IsNotNull(result.Metadata);
        Assert.AreEqual("https://urlbox.com", result.Metadata.UrlRequested);
        Assert.AreEqual("https://example.com", result.Metadata.UrlResolved);
        Assert.AreEqual("Example Title", result.Metadata.Title);
    }

    [TestMethod]
    public async Task GetStatus_succeeds()
    {
        string renderId = "ca482d7e-9417-4569-90fe-80f7c5e1c781";
        string statusResponse = @"
        {
            ""status"": ""succeeded"",
            ""renderId"": ""ca482d7e-9417-4569-90fe-80f7c5e1c781"",
            ""renderUrl"": ""https://example.com/screenshot.png"",
            ""size"": 123456,
            ""metadata"": {
                ""urlRequested"": ""https://urlbox.com"",
                ""url"": ""https://urlbox.com"",
                ""urlResolved"": ""https://example.com"",
                ""title"": ""Example Title""
            }
        }";

        client.StubRequest(
            HttpMethod.Get,
            $"{Urlbox.BASE_URL}/v1/render/ca482d7e-9417-4569-90fe-80f7c5e1c781",
            (HttpStatusCode)200,
            statusResponse
        );

        AsyncUrlboxResponse status = await urlbox.GetStatus(renderId);

        Assert.AreEqual(status.RenderId, renderId);
        Assert.IsNotNull(status.Status);
        Assert.AreEqual(status.Status, "succeeded");
    }

    [TestMethod]
    public async Task GetStatus_fails()
    {
        string renderId = "ca482d7e-9417-4569-90fe-80f7c5e1c781";

        client.StubRequest(
            HttpMethod.Get,
            $"{Urlbox.BASE_URL}/v1/render/{renderId}",
            (HttpStatusCode)500,
            "" // No response body or error headers
        );

        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(
            async () => await urlbox.GetStatus(renderId)
        );

        Assert.AreEqual(
            "Failed to check status of async request: Request failed: No x-urlbox-error-message header found",
            exception.Message
        );
    }

    [TestMethod]
    public async Task TestDownloadToFile_succeeds()
    {
        var urlboxUrl = "https://api.urlbox.com/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/5ee277f206869517d00cf1951f30d48ef9c64bfe/png?url=google.com";

        client.StubRequest(
            HttpMethod.Get,
            urlboxUrl,
            (HttpStatusCode)200,
            "somebuffer" // No response body or error headers
        );

        var result = await urlbox.DownloadToFile(urlboxUrl, "result.png");
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(String));
        Assert.IsTrue(result.Length >= 0);
    }

    [TestMethod]
    public async Task TestDownloadToFile_fails()
    {
        string urlboxUrl = "https://api.urlbox.com/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/5ee277f206869517d00cf1951f30d48ef9c64bfe/png?url=google.com";
        client.StubRequest(
            HttpMethod.Get,
            urlboxUrl,
            (HttpStatusCode)400,
            "",
            headers: new Dictionary<string, string>
            {
                    { "x-urlbox-error-message", "some error message from Urlbox API" }
            }
        );

        var result = await Assert.ThrowsExceptionAsync<System.Exception>(async () => await urlbox.DownloadToFile(urlboxUrl, "result.png"));

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Message, "Request failed: some error message from Urlbox API");
    }

    [TestMethod]
    public async Task TestDownloadBase64()
    {
        string urlboxUrl = "https://api.urlbox.com/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/59148a4e454a2c7051488defdb8b246bdea61ace/jpeg?url=bbc.co.uk";
        string mockContent = "Test Image Content";

        string encodedContent = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(mockContent));
        string expectedBase64 = "text/plain; charset=utf-8;base64," + encodedContent;

        client.StubRequest(
            HttpMethod.Get,
            urlboxUrl,
            HttpStatusCode.OK,
            mockContent
        );


        string base64result = await urlbox.DownloadAsBase64(urlboxUrl);

        Assert.IsNotNull(base64result);
        Assert.AreEqual(expectedBase64, base64result, "Expected the base64 string to match the mocked content.");
    }

    [TestMethod]
    public async Task TestDownloadFail()
    {
        string urlboxUrl = "https://api.urlbox.com/v1/ca482d7e-9417-4569-90fe-80f7c5e1c781/59148a4e454a2c7051488defdb8b246bdea61ac/jpeg?url=bbc.co.uk";
        string expectedErrorMessage = "The generated token was incorrect. Please look in the docs (https://urlbox.io/docs) for how to generate your token correctly in the language you are using. TLDR: It should be the HMAC SHA256 of your query string, *signed* by your user secret, which you can find by logging into the urlbox dashboard>. Expected the error message to match the mocked content.";

        client.StubRequest(
            HttpMethod.Get,
            urlboxUrl,
            HttpStatusCode.Unauthorized,
            "",
            new Dictionary<string, string>
            {
            { "x-urlbox-error-message", expectedErrorMessage }
            }
        );

        var exception = await Assert.ThrowsExceptionAsync<System.Exception>(
            async () => await urlbox.DownloadAsBase64(urlboxUrl)
        );

        Assert.IsNotNull(exception);
        Assert.AreEqual("Request failed: " + expectedErrorMessage, exception.Message, "Expected the error message to match the mocked content.");
    }
}
