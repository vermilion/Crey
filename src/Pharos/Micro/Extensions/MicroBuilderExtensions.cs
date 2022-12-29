using Microsoft.Extensions.DependencyInjection;
using Psi.BackgroundServices;
using Psi.Builder.Abstractions;
using Psi.Micro;
using Psi.Micro.Abstractions;
using Psi.Proxy;
using Psi.Proxy.Proxy;
using Psi.Proxy.Abstractions;
using Psi.ServiceDiscovery.Models;

namespace Psi.Micro.Extensions;

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
