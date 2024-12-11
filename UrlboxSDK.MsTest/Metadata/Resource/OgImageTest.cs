using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Metadata.Resource;

namespace UrlboxSDK.MsTest.Metadata.Resource;

[TestClass]
public class OgImageTests
{
    [TestMethod]
    public void OgImage_CreatesGetters()
    {
        OgImage ogImage = new(
            url: "url",
            type: "type",
            width: "123",
            height: "123"
        );
        Assert.IsInstanceOfType(ogImage, typeof(OgImage));
        Assert.AreEqual("url", ogImage.Url);
        Assert.AreEqual("type", ogImage.Type);
        Assert.AreEqual("123", ogImage.Width);
        Assert.AreEqual("123", ogImage.Height);
    }
}
