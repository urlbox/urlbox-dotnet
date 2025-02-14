using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UrlboxSDK.MsTest.Resource;

[TestClass]
public class UrlboxRegionTest
{
    [TestMethod]
    public void Baseurl_NotSet()
    {
        Urlbox fromCredentials = Urlbox.FromCredentials("MY_API_KEY", "secret", "webhook_secret");
        Assert.IsInstanceOfType(fromCredentials, typeof(Urlbox));
        Urlbox fromNew = Urlbox.FromCredentials("MY_API_KEY", "secret", "webhook_secret");
        Assert.IsInstanceOfType(fromNew, typeof(Urlbox));
    }

    [TestMethod]
    public void Baseurl_included()
    {
        Urlbox fromCredentials = Urlbox.FromCredentials("MY_API_KEY", "secret", "webhook_secret", "someBaseUrl");
        Assert.IsInstanceOfType(fromCredentials, typeof(Urlbox));
        Urlbox fromNew = Urlbox.FromCredentials("MY_API_KEY", "secret", "webhook_secret", "someBaseUrl");
        Assert.IsInstanceOfType(fromNew, typeof(Urlbox));
    }
}
