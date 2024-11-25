using System;
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
}
