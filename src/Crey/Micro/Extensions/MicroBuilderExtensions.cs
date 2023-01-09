using Crey.BackgroundServices;
using Crey.Builder.Abstractions;
using Crey.Micro.Abstractions;
using Crey.Proxy;
using Crey.Proxy.Abstractions;
using Crey.ServiceDiscovery.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Micro.Extensions;

public static class ServiceCollectionExtensions
{
    public static IMicroBuilder AddMicroClient(this IMicroBuilder builder, Action<IMicroClientBuilder> builderAction = null)
    {
        var services = builder.Services;

        builderAction?.Invoke(builder);

        services.AddBase();

        // proxy services
        services.AddSingleton<ClientProxyInterceptor>();
        services.AddSingleton<IProxyFactory, ProxyFactory>();
        services.AddScoped<IProxyProvider, ClientProxy>();

        return builder;
    }

    public static IMicroBuilder AddMicroService(this IMicroBuilder builder, Action<IMicroServerBuilder> builderAction)
    {
        var services = builder.Services;

        builderAction.Invoke(builder);

        services.AddBase();
        services.AddSingleton<IMicroHost, MicroHost>();

        services.AddHostedService<HostListenerBackroungService>();

        services.Configure<ServiceAddress>(builder.ConfigurationSection.GetSection("service"));

        return builder;
    }

    private static void AddBase(this IServiceCollection services)
    {
        services.AddSingleton(services);
        services.AddScoped<IMicroSession, MicroSession>();
        services.AddScoped<IMicroExecutor, MicroExecutor>();
        services.AddSingleton<IMicroEntryFactory, MicroEntryFactory>();
    }
}
