using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UrlboxSDK.MsTest.DI;

[TestClass]
public class DependencyInjectionTests
{
    [TestMethod]
    public void ShouldResolveIUrlboxAsSingleton()
    {
        var services = new ServiceCollection();

        // Register IUrlbox as a singleton with the DI container
        services.AddSingleton<IUrlbox>(provider => new Urlbox("key", "secret", "webhookSecret"));

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        var instance1 = serviceProvider.GetService<IUrlbox>();
        var instance2 = serviceProvider.GetService<IUrlbox>();

        Assert.IsNotNull(instance1, "IUrlbox instance should not be null");
        Assert.AreSame(instance1, instance2, "DI should return the same singleton instance");
    }

    [TestMethod]
    public void ShouldResolveIUrlboxAsScoped()
    {
        var services = new ServiceCollection();

        // Register IUrlbox as a scoped service
        services.AddScoped<IUrlbox>(provider => new Urlbox("key", "secret", "webhookSecret"));

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();
        var instance1 = scope1.ServiceProvider.GetService<IUrlbox>();
        var instance2 = scope2.ServiceProvider.GetService<IUrlbox>();

        Assert.IsNotNull(instance1, "Instance in scope1 should not be null");
        Assert.IsNotNull(instance2, "Instance in scope2 should not be null");
        Assert.AreNotSame(instance1, instance2, "Scoped instances should be unique to each scope");
    }

    [TestMethod]
    public void ShouldResolveIUrlboxAsTransient()
    {
        var services = new ServiceCollection();

        // Register IUrlbox as a transient service
        services.AddTransient<IUrlbox>(provider => new Urlbox("key", "secret", "webhookSecret"));

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        var instance1 = serviceProvider.GetService<IUrlbox>();
        var instance2 = serviceProvider.GetService<IUrlbox>();

        Assert.IsNotNull(instance1, "First transient instance should not be null");
        Assert.IsNotNull(instance2, "Second transient instance should not be null");
        Assert.AreNotSame(instance1, instance2, "Transient instances should be unique on each resolution");
    }
}
