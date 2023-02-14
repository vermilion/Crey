using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Discovery.Consul;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Consul as Service Discovery provider
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <param name="action">Action to configure options</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddConsulDiscovery(this IMicroBuilder builder, Action<ConsulOptions>? action = null)
    {
        var services = builder.Services;

        services.Configure<ConsulOptions>(options =>
        {
            builder.ConfigurationSection.GetSection("Discovery:Consul").Bind(options);
            action?.Invoke(options);
        });

        services.AddSingleton<IServiceRegister, ConsulServiceRegister>();
        services.AddSingleton<IServiceFinder, ConsulServiceFinder>();

        return builder;
    }
}
