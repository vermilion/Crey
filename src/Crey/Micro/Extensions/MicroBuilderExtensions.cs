using Microsoft.Extensions.DependencyInjection;
using Crey.BackgroundServices;
using Crey.Builder.Abstractions;
using Crey.Micro;
using Crey.Micro.Abstractions;
using Crey.Proxy;
using Crey.Proxy.Abstractions;
using Crey.ServiceDiscovery.Models;
using Crey.Proxy.ProxyGenerator;

namespace Crey.Micro.Extensions;

public static class ServiceCollectionExtensions
{
    public static IMicroBuilder AddMicroClient(this IMicroBuilder builder, Action<IMicroClientBuilder> builderAction = null)
    {
        var services = builder.Services;

        builderAction?.Invoke(builder);

        services.AddBase();

        // proxy services
        services.AddSingleton<AsyncProxyGenerator>();
        services.AddScoped<IProxyProvider, ClientProxy>();
        services.AddScoped<IProxyFactory, ProxyFactory>();

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
