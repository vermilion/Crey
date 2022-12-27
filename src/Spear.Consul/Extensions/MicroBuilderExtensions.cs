using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spear.Core.Builder.Abstractions;
using Spear.Core.ServiceDiscovery.Abstractions;
using Spear.Discovery.Consul.Options;

namespace Spear.Discovery.Consul.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Consul as Service Discovery provider
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <param name="optionAction">Action to configure options</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddConsulDiscovery(this IMicroBuilder builder, Action<ConsulOptions> optionAction = null)
    {
        builder.Services.Configure<ConsulOptions>(builder.Configuration.GetSection("consul"));

        builder.Services.AddSingleton<IServiceRegister>(provider =>
        {
            var option = provider.GetRequiredService<IOptions<ConsulOptions>>().Value;
            optionAction?.Invoke(option);
            var logger = provider.GetRequiredService<ILogger<ConsulServiceRegister>>();

            return new ConsulServiceRegister(logger, option.Server, option.Token);
        });

        builder.Services.AddSingleton<IServiceFinder>(provider =>
        {
            var option = provider.GetRequiredService<IOptions<ConsulOptions>>().Value;
            optionAction?.Invoke(option);
            var logger = provider.GetRequiredService<ILogger<ConsulServiceFinder>>();

            return new ConsulServiceFinder(logger, option.Server, option.Token);
        });

        return builder;
    }
}
