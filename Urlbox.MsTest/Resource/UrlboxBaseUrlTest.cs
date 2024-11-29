using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using UrlboxSDK;

[TestClass]
public class UrlboxRegionTest
{
    private Urlbox urlbox;
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
    }


    [TestMethod]
    public void Baseurl_NotSet()
    {
        var fromCredentials = Urlbox.FromCredentials("MY_API_KEY", "secret", "webhook_secret");
        Assert.IsInstanceOfType(fromCredentials, typeof(Urlbox));
        var fromNew = Urlbox.FromCredentials("MY_API_KEY", "secret", "webhook_secret");
        Assert.IsInstanceOfType(fromNew, typeof(Urlbox));
    }

    [TestMethod]
    public void Baseurl_included()
    {
        var fromCredentials = Urlbox.FromCredentials("MY_API_KEY", "secret", "webhook_secret", "someBaseUrl");
        Assert.IsInstanceOfType(fromCredentials, typeof(Urlbox));
        var fromNew = Urlbox.FromCredentials("MY_API_KEY", "secret", "webhook_secret", "someBaseUrl");
        Assert.IsInstanceOfType(fromNew, typeof(Urlbox));
    }
}
