using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.DI.Resource;

namespace UrlboxSDK.MsTest.DI.Resource
{
    [TestClass]
    public class UrlboxConfigTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UrlboxConfig_ThrowsArgumentException_WhenKeyIsMissing()
        {
            UrlboxConfig config = new()
            {
                Secret = "valid-secret"
            };

            config.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UrlboxConfig_ThrowsArgumentException_WhenSecretIsMissing()
        {
            UrlboxConfig config = new()
            {
                Key = "valid-key"
            };

            config.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UrlboxConfig_ThrowsArgumentException_WhenBothKeyAndSecretAreMissing()
        {
            UrlboxConfig config = new();

            config.Validate();
        }

        [TestMethod]
        public void UrlboxConfig_CreatesInstance_WhenWebhookSecretIsNotProvided()
        {
            UrlboxConfig config = new()
            {
                Key = "valid-key",
                Secret = "valid-secret"
            };

            config.Validate();

            Assert.IsNotNull(config);
            Assert.AreEqual("valid-key", config.Key);
            Assert.AreEqual("valid-secret", config.Secret);
            Assert.IsNull(config.WebhookSecret);
            Assert.AreEqual(Urlbox.BASE_URL, config.BaseUrl);
        }

        [TestMethod]
        public void UrlboxConfig_CreatesInstance_WhenWebhookSecretIsProvided()
        {
            UrlboxConfig config = new()
            {
                Key = "valid-key",
                Secret = "valid-secret",
                WebhookSecret = "webhook-secret"
            };

            config.Validate();

            Assert.IsNotNull(config);
            Assert.AreEqual("valid-key", config.Key);
            Assert.AreEqual("valid-secret", config.Secret);
            Assert.AreEqual("webhook-secret", config.WebhookSecret);
            Assert.AreEqual(Urlbox.BASE_URL, config.BaseUrl);
        }
    }
}