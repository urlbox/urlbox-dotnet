#nullable enable

using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK;

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
}
