using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.Factory;

namespace UrlboxSDK.MSTest.Factory
{
    [TestClass]
    public class UrlboxFactoryTests
    {
        private IUrlboxFactory factory;

        [TestInitialize]
        public void Setup()
        {
            factory = new UrlboxFactory();
        }

        [TestMethod]
        public void Create_ShouldReturnInstanceOfIUrlbox()
        {
            string key = "test-key";
            string secret = "test-secret";

            var result = factory.Create(key, secret);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IUrlbox));
        }

        [TestMethod]
        public void Create_WithWebhookSecret_ShouldReturnValidInstance()
        {
            string key = "test-key";
            string secret = "test-secret";
            string webhookSecret = "test-webhook-secret";

            var result = factory.Create(key, secret, webhookSecret);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IUrlbox));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_ShouldThrowException_WhenKeyIsNull()
        {
            factory.Create(null!, "test-secret");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_ShouldThrowException_WhenSecretIsNull()
        {
            factory.Create("test-key", null!);
        }
    }
}