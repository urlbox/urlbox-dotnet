using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Factory;
using UrlboxSDK.Options.Resource;
using System;

namespace UrlboxSDK.MSTest.Factory
{
    [TestClass]
    public class RenderLinkFactoryTests
    {
        private RenderLinkFactory renderLinkFactory;
        private const string BaseUrl = "https://api.urlbox.com";
        private const string TestKey = "test-key";
        private const string TestSecret = "test-secret";

        [TestInitialize]
        public void Setup()
        {
            renderLinkFactory = new RenderLinkFactory(TestKey, TestSecret);
        }

        [TestMethod]
        public void GenerateRenderLink_SignTrue_ShouldReturnSignedUrl()
        {
            var options = new UrlboxOptions(url: "https://example.com");
            string expectedQueryString = "url=https%3A%2F%2Fexample.com";
            string expectedLinkUnsigned = $"{BaseUrl}/v1/{TestKey}/png?{expectedQueryString}";

            string result = renderLinkFactory.GenerateRenderLink(BaseUrl, options, sign: true);

            Assert.IsTrue(result.Contains(expectedQueryString));
            Assert.IsTrue(result != expectedLinkUnsigned);
        }

        [TestMethod]
        public void GenerateRenderLink_SignFalse_ShouldReturnUnsignedUrl()
        {
            var options = new UrlboxOptions(url: "https://example.com");
            string expectedQueryString = "url=https%3A%2F%2Fexample.com";
            string expectedLink = $"{BaseUrl}/v1/{TestKey}/png?{expectedQueryString}";
            string result = renderLinkFactory.GenerateRenderLink(BaseUrl, options, sign: false);

            Assert.AreEqual(expectedLink, result);
        }

        [TestMethod]
        public void GenerateRenderLink_WithDiffFormatFormat_ShouldReturnExpectedLink()
        {
            var options = Urlbox.Options(url: "https://example.com").Format(Format.Jpeg).Build();

            string expectedQueryString = "url=https%3A%2F%2Fexample.com";
            string expectedLink = $"{BaseUrl}/v1/{TestKey}/jpeg?{expectedQueryString}";

            string result = renderLinkFactory.GenerateRenderLink(BaseUrl, options, sign: false);

            Assert.AreEqual(expectedLink, result);
        }
    }
}