using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Discovery.StaticList;

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

        services.Configure<StaticListOptions>(options =>
        {
            builder.ConfigurationSection.GetSection("Discovery:Static").Bind(options);
            action?.Invoke(options);
        });

        services.AddSingleton<StaticListServiceFinder>();
        services.AddSingleton<IServiceRegister>(provider => provider.GetRequiredService<StaticListServiceFinder>());
        services.AddSingleton<IServiceFinder>(provider => provider.GetRequiredService<StaticListServiceFinder>());
        return builder;
    }
}
