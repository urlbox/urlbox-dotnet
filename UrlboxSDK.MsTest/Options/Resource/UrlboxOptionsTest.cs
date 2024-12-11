#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Options.Resource;

namespace UrlboxSDK.MsTest.Options.Resource;

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
        ArgumentException exception = Assert.ThrowsException<ArgumentException>(() => new UrlboxOptions(url: "urlbox.com", html: "<h1>test</h1>"));
        Assert.AreEqual(exception.Message, "Either but not both options 'url' or 'html' must be provided.");
    }

    [TestMethod]
    public void UrlboxOptions_CreatesSuccess_URL()
    {
        string url = "https://urlbox.com";
        UrlboxOptions urlboxOptions = new(url: url);

        Assert.IsNotNull(urlboxOptions);
        Assert.IsInstanceOfType(urlboxOptions, typeof(UrlboxOptions));
        Assert.AreEqual(url, urlboxOptions.Url);
        Assert.IsNull(urlboxOptions.Html);
    }

    [TestMethod]
    public void UrlboxOptions_CreatesSuccess_HTML()
    {
        string html = "<h1>test</h1>";
        UrlboxOptions urlboxOptions = new(html: html);

        Assert.IsNotNull(urlboxOptions);
        Assert.IsInstanceOfType(urlboxOptions, typeof(UrlboxOptions));
        Assert.AreEqual(html, urlboxOptions.Html);
        Assert.IsNull(urlboxOptions.Url);
    }

    /// <summary>
    /// Tests that you can dynamically assign options on construct
    /// </summary>
    [TestMethod]
    public void UrlboxOptions_CreatedOnInit()
    {
        string html = "<h1>test</h1>";
        UrlboxOptions urlboxOptions = new(html: html)
        {
            Format = Format.Pdf
        };

        Assert.IsTrue(urlboxOptions.Format == Format.Pdf);
    }
}
