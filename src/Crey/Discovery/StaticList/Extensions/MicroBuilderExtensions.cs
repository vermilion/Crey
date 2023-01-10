using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Crey.Builder;

namespace Crey.Discovery.StaticRouter;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds StaticList discovery as default
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <param name="action">Action to configure options</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddStaticListDiscovery(this IMicroBuilder builder, Action<StaticListOptions>? action = null)
    {
        var services = builder.Services;

        services.AddSingleton(provider =>
        {
            var options = builder.ConfigurationSection.GetSection("discovery:static").Get<StaticListOptions>() ?? new StaticListOptions();
            action?.Invoke(options);

            return Microsoft.Extensions.Options.Options.Create(options);
        });

        services.AddSingleton<StaticListServiceFinder>();
        services.AddSingleton<IServiceRegister>(provider => provider.GetRequiredService<StaticListServiceFinder>());
        services.AddSingleton<IServiceFinder>(provider => provider.GetRequiredService<StaticListServiceFinder>());
        return builder;
    }
}
