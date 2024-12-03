using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK;
using UrlboxSDK;

[TestClass]
public class UrlboxRegionTest
{
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
