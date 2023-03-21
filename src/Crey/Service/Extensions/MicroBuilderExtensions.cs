using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Crey.Service;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds service-related services
    /// </summary>
    /// <param name="builder">Builder instance <see cref="IMicroBuilder"/></param>
    /// <param name="builderAction">Configure action</param>
    /// <returns>Fluent for <see cref="IMicroBuilder"/></returns>
    public static IMicroBuilder AddMicroService(this IMicroBuilder builder, Action<IMicroServerBuilder> builderAction)
    {
        var services = builder.Services;

        services.AddCoreServices();

        services.AddSingleton<IServiceHost, ServiceHost>();
        services.AddSingleton<IServiceMethodExecutor, ServiceMethodExecutor>();
        services.AddSingleton<IServiceEntryFactory, ServiceEntryFactory>();

        services.AddScoped<ICallContextAccessor, CallContextAccessor>();

        services.AddHostedService<HostListenerBackgroundService>();

        services.Configure<ServiceAddress>(builder.ConfigurationSection.GetSection("Service"));
        services.AddTransient<IPostConfigureOptions<ServiceAddress>, PostConfigureServiceAddress>();

        builder.AddServiceDiscoveryServices();

        builderAction.Invoke(builder);

        return builder;
    }
}
