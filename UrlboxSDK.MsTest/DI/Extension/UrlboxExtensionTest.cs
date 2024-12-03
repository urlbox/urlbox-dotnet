using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlboxSDK.DI.Extension;
using UrlboxSDK.DI.Resource;
using UrlboxSDK.Resource;

namespace UrlboxSDK.MsTest.DI.Extension
{
    [TestClass]
    public class UrlboxExtensionTest
    {
        /// <summary>
        /// Tests registering the UrlboxConfig obj in Service Container as an IOptions
        /// </summary>
        [TestMethod]
        public void AddUrlbox_RegistersUrlboxConfig()
        {
            ServiceCollection services = new();
            string apiKey = "test-key";
            string apiSecret = "test-secret";

            services.AddUrlbox(options =>
            {
                options.Key = apiKey;
                options.Secret = apiSecret;
                options.WebhookSecret = "test-webhook";
                options.BaseUrl = "https://test-urlbox.com";
            });
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            UrlboxConfig options = serviceProvider.GetRequiredService<IOptions<UrlboxConfig>>().Value;
            Assert.AreEqual(apiKey, options.Key);
            Assert.AreEqual(apiSecret, options.Secret);
            Assert.AreEqual("test-webhook", options.WebhookSecret);
            Assert.AreEqual("https://test-urlbox.com", options.BaseUrl);
        }

        /// <summary>
        /// Tests that AddUrlbox adds an instance of Urlbox with the default lifetime
        /// </summary>
        [TestMethod]
        public void AddUrlbox_RegistersUrlboxService()
        {
            ServiceCollection services = new();
            string apiKey = "test-key";
            string apiSecret = "test-secret";

            services.AddUrlbox(options =>
            {
                options.Key = apiKey;
                options.Secret = apiSecret;
                options.WebhookSecret = "test-webhook";
                options.BaseUrl = "https://test-urlbox.com";
            });
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ServiceDescriptor descriptor = services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IUrlbox));
            Assert.IsNotNull(descriptor, "IUrlbox service is not registered.");
            Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime, "The registered lifetime is not the default Singleton.");

            IUrlbox urlboxService = serviceProvider.GetRequiredService<IUrlbox>();
            Assert.IsNotNull(urlboxService);
            Assert.IsInstanceOfType(urlboxService, typeof(Urlbox));
        }

        [TestMethod]
        public void AddUrlbox_CreatesUrlboxInstanceWithCorrectConfig()
        {
            ServiceCollection services = new();
            string apiKey = "test-key";
            string apiSecret = "test-secret";

            services.AddUrlbox(options =>
            {
                options.Key = apiKey;
                options.Secret = apiSecret;
                options.WebhookSecret = "test-webhook";
                options.BaseUrl = "https://test-urlbox.com";
            });
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IUrlbox urlbox = (Urlbox)serviceProvider.GetRequiredService<IUrlbox>();

            string jpgUrl = urlbox.GenerateJPEGUrl(Urlbox.Options(url: "https://urlbox.com").Build());

            Assert.IsTrue(jpgUrl.Contains("test-key"));
        }

        [TestMethod]
        public void AddUrlbox_RegistersAsSingleton()
        {
            ServiceCollection services = new();
            string apiKey = "test-key";
            string apiSecret = "test-secret";

            services.AddUrlbox(options =>
            {
                options.Key = apiKey;
                options.Secret = apiSecret;
            }, ServiceLifetime.Singleton);
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ServiceDescriptor descriptor = services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IUrlbox));
            Assert.IsNotNull(descriptor, "IUrlbox service is not registered.");
            Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime, "The registered lifetime is not Singleton.");

            IUrlbox instance1 = serviceProvider.GetRequiredService<IUrlbox>();
            IUrlbox instance2 = serviceProvider.GetRequiredService<IUrlbox>();
            Assert.AreSame(instance1, instance2); // Singleton instances should be the same
        }

        [TestMethod]
        public void AddUrlbox_RegistersAsScoped()
        {
            ServiceCollection services = new();
            string apiKey = "test-key";
            string apiSecret = "test-secret";

            services.AddUrlbox(options =>
            {
                options.Key = apiKey;
                options.Secret = apiSecret;
            }, ServiceLifetime.Scoped);
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ServiceDescriptor descriptor = services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IUrlbox));
            Assert.IsNotNull(descriptor, "IUrlbox service is not registered.");
            Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime, "The registered lifetime is not scoped.");

            using (IServiceScope scope1 = serviceProvider.CreateScope())
            {
                IUrlbox instance1 = scope1.ServiceProvider.GetRequiredService<IUrlbox>();
                IUrlbox instance2 = scope1.ServiceProvider.GetRequiredService<IUrlbox>();
                Assert.AreSame(instance1, instance2); // Scoped instances within the same scope should be the same
            }

            using (IServiceScope scope2 = serviceProvider.CreateScope())
            {
                IUrlbox instance3 = scope2.ServiceProvider.GetRequiredService<IUrlbox>();
                Assert.AreNotSame(instance3, serviceProvider.GetRequiredService<IUrlbox>()); // Scoped instances across scopes should differ
            }
        }

        [TestMethod]
        public void AddUrlbox_RegistersAsTransient()
        {
            ServiceCollection services = new();
            string apiKey = "test-key";
            string apiSecret = "test-secret";

            services.AddUrlbox(options =>
            {
                options.Key = apiKey;
                options.Secret = apiSecret;
            }, ServiceLifetime.Transient);
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ServiceDescriptor descriptor = services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IUrlbox));
            Assert.IsNotNull(descriptor, "IUrlbox service is not registered.");
            Assert.AreEqual(ServiceLifetime.Transient, descriptor.Lifetime, "The registered lifetime is not transient.");


            var instance1 = serviceProvider.GetRequiredService<IUrlbox>();
            var instance2 = serviceProvider.GetRequiredService<IUrlbox>();
            Assert.AreNotSame(instance1, instance2); // Transient instances should always differ
        }
    }
}