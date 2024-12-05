using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UrlboxSDK.DI.Resource;
using UrlboxSDK;

namespace UrlboxSDK.DI.Extension;
/// <summary>
/// Provides extension methods for registering Urlbox services in the dependency injection container.
/// </summary>
public static class UrlboxExtension
{
    /// <summary>
    /// Registers the Urlbox class and its configuration in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="config">An action to configure the <see cref="UrlboxConfig"/> options.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddUrlbox(
        this IServiceCollection services,
        Action<UrlboxConfig> configure,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        // Add config to ServiceProvider
        services.Configure(configure);

        // Register Urlbox service with lifetime
        services.Add(new ServiceDescriptor(typeof(IUrlbox), serviceProvider =>
        {
            UrlboxConfig options = serviceProvider.GetRequiredService<IOptions<UrlboxConfig>>().Value;

            options.Validate();

            return new Urlbox(options.Key!, options.Secret!, options.WebhookSecret, options.BaseUrl);
        }, lifetime));
        return services;
    }
}
