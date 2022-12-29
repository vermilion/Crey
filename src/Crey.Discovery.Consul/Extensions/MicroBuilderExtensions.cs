using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Crey.Builder.Abstractions;
using Crey.Discovery.Consul;
using Crey.Discovery.Consul.Options;
using Crey.ServiceDiscovery.Abstractions;

namespace Crey.Discovery.Consul.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Consul as Service Discovery provider
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <param name="action">Action to configure options</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddConsulDiscovery(this IMicroBuilder builder, Action<ConsulOptions> action = null)
    {
        var services = builder.Services;

        services.AddSingleton(provider =>
        {
            var options = builder.ConfigurationSection.GetSection("discovery:consul").Get<ConsulOptions>() ?? new ConsulOptions();
            action?.Invoke(options);

            return Microsoft.Extensions.Options.Options.Create(options);
        });

        services.AddSingleton<IServiceRegister, ConsulServiceRegister>();
        services.AddSingleton<IServiceFinder, ConsulServiceFinder>();

        return builder;
    }
}
