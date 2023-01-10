using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Crey.Builder;
using Crey.ServiceDiscovery.Abstractions;
using Crey.ServiceDiscovery.StaticRouter;
using Crey.ServiceDiscovery.StaticRouter.Options;

namespace Crey.ServiceDiscovery.StaticRouter.Extensions;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds Static discovery as default
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <param name="action">Action to configure options</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddStaticServiceDiscovery(this IMicroBuilder builder, Action<StaticRouterOptions> action = null)
    {
        var services = builder.Services;

        services.AddSingleton(provider =>
        {
            var options = builder.ConfigurationSection.GetSection("discovery:static").Get<StaticRouterOptions>() ?? new StaticRouterOptions();
            action?.Invoke(options);

            return Microsoft.Extensions.Options.Options.Create(options);
        });

        services.AddSingleton<StaticServiceRouter>();
        services.AddSingleton<IServiceRegister>(provider => provider.GetRequiredService<StaticServiceRouter>());
        services.AddSingleton<IServiceFinder>(provider => provider.GetRequiredService<StaticServiceRouter>());
        return builder;
    }
}
