#nullable enable

using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK;
using UrlboxSDK.Options.Resource;
using UrlboxSDK.Policy;

[TestClass]
public class UrlboxOptionsTest
{
    [TestMethod]
    public void UrlboxOptions_MissingHTMLandURL()
    {
        Assert.ThrowsException<ArgumentException>(() => new UrlboxOptions());
    }

    [TestMethod]
    public void UrlboxOptions_BothHTMLandURL()
    {
        var exception = Assert.ThrowsException<ArgumentException>(() => new UrlboxOptions(url: "urlbox.com", html: "<h1>test</h1>"));
        Assert.AreEqual(exception.Message, "Either but not both options 'url' or 'html' must be provided.");
    }

    [TestMethod]
    public void UrlboxOptions_CreatesSuccess_URL()
    {
        string url = "https://urlbox.com";
        var urlboxOptions = new UrlboxOptions(url: url);

        Assert.IsNotNull(urlboxOptions);
        Assert.IsInstanceOfType(urlboxOptions, typeof(UrlboxOptions));
        Assert.AreEqual(url, urlboxOptions.Url);
        Assert.IsNull(urlboxOptions.Html);
    }

    [TestMethod]
    public void UrlboxOptions_CreatesSuccess_HTML()
    {
        string html = "<h1>test</h1>";
        var urlboxOptions = new UrlboxOptions(html: html);

        Assert.IsNotNull(urlboxOptions);
        Assert.IsInstanceOfType(urlboxOptions, typeof(UrlboxOptions));
        Assert.AreEqual(html, urlboxOptions.Html);
        Assert.IsNull(urlboxOptions.Url);
    }

    [TestMethod]
    public void UrlboxOptions_PassingWrongTypeCookie()
    {
        string html = "<h1>test</h1>";
        var urlboxOptions = new UrlboxOptions(html: html);

        var exception = Assert.ThrowsException<ArgumentException>(() => urlboxOptions.Cookie = 1);
        Assert.IsTrue(exception.Message.Contains("Cookie must be either a string or a string array."));
    }

    [TestMethod]
    public void UrlboxOptions_PassingWrongTypeHeader()
    {
        string html = "<h1>test</h1>";
        var urlboxOptions = new UrlboxOptions(html: html);

        var exception = Assert.ThrowsException<ArgumentException>(() => urlboxOptions.Header = 1);
        Assert.IsTrue(exception.Message.Contains("Header must be either a string or a string array."));
    }

    /// <summary>
    /// Tests that you can dynamically assign options on construct
    /// </summary>
    [TestMethod]
    public void UrlboxOptions_CreatedOnInit()
    {
        string html = "<h1>test</h1>";
        UrlboxOptions urlboxOptions = new UrlboxOptions(html: html)
        {
            FullPage = true
        };

        Assert.IsTrue(urlboxOptions.FullPage);
    }

    /// <summary>
    /// Tests the string validation for platform
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="expectation"></param>
    [TestMethod]
    [DataRow("Linux armv81", "Linux armv81")]
    [DataRow("Linux x86_64", "Linux x86_64")]
    [DataRow("Win32", "Win32")]
    [DataRow("MacIntel", "MacIntel")]
    [DataRow("something not acceptable", null)]
    public void UrlboxOptions_CreatedWithPlatforms(
        string platform,
        string? expectation
    )
    {
        if (expectation == null)
        {
            Assert.ThrowsException<ArgumentException>(() => Urlbox.Options(url: "https://urlbox.com")
            .Platform(platform)
            .Build());
        }
        else
        {
            UrlboxOptions options = Urlbox.Options(url: "https://urlbox.com")
            .Platform(platform)
            .Build();
            JsonSerializerOptions serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                WriteIndented = true
            };

            Assert.AreEqual(platform, options.Platform);
            string serialized = JsonSerializer.Serialize(options, serializeOptions);
            Assert.IsTrue(serialized.Contains(expectation));
        }
    }

    [TestMethod]
    public void Quality_ShouldThrowException_WhenOutOfRange()
    {
        var options = new UrlboxOptions(url: "https://urlbox.com");

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => options.Quality = -1, "Quality must be between 0 and 100.");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => options.Quality = 101, "Quality must be between 0 and 100.");
    }

    [TestMethod]
    public void Quality_ShouldAcceptValidValues()
    {
        var options = new UrlboxOptions(url: "https://urlbox.com");

        options.Quality = 0;
        Assert.AreEqual(0, options.Quality);
        options.Quality = 50;
        Assert.AreEqual(50, options.Quality);
        options.Quality = 100;
        Assert.AreEqual(100, options.Quality);
    }

    [TestMethod]
    public void PdfScale_ShouldThrowException_WhenOutOfRange()
    {
        var options = new UrlboxOptions(url: "https://urlbox.com");

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => options.PdfScale = 0.09, "PdfScale must be between 0 and 100.");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => options.PdfScale = 2.01, "PdfScale must be between 0 and 100.");
    }

    [TestMethod]
    public void PdfScale_ShouldAcceptValidValues()
    {
        var options = new UrlboxOptions(url: "https://urlbox.com");

        options.PdfScale = 0.1;
        Assert.AreEqual(0.1, options.PdfScale);
        options.PdfScale = 1.2;
        Assert.AreEqual(1.2, options.PdfScale);
        options.PdfScale = 2.0;
        Assert.AreEqual(2.0, options.PdfScale);
    }
}
