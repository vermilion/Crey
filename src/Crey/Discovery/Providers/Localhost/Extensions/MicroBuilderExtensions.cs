using Microsoft.Extensions.DependencyInjection;

namespace Crey.Discovery.Localhost;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds StaticList discovery as default
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddLocalhostDiscovery(this IMicroBuilder builder, Action<ServiceAddress>? action = null)
    {
        var services = builder.Services;

        services.Configure<ServiceAddress>(options =>
        {
            var address = new ServiceAddress();
            action?.Invoke(options);
        });

        services.AddSingleton<LocalhostServiceFinder>();
        services.AddSingleton<IServiceRegister>(provider => provider.GetRequiredService<LocalhostServiceFinder>());
        services.AddSingleton<IServiceFinder>(provider => provider.GetRequiredService<LocalhostServiceFinder>());
        return builder;
    }
}
