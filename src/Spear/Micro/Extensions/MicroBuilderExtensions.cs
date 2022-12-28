using Microsoft.Extensions.DependencyInjection;
using Spear.Core.BackgroundServices;
using Spear.Core.Builder.Abstractions;
using Spear.Core.Micro;
using Spear.Core.Micro.Abstractions;
using Spear.Core.Proxy;
using Spear.Core.ServiceDiscovery.Models;
using Spear.Micro.Abstractions;
using Spear.ProxyGenerator.Abstractions;
using Spear.ProxyGenerator;
using Spear.ProxyGenerator.Proxy;

namespace Spear.Micro.Extensions;

public static class ServiceCollectionExtensions
{
    public static IMicroBuilder AddMicroClient(this IMicroBuilder builder, Action<IMicroClientBuilder> builderAction)
    {
        var services = builder.Services;

        builderAction.Invoke(builder);

        services.AddScoped<IMicroSession, MicroSession>();
        
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

        services.AddSingleton(services);
        services.AddSingleton<IMicroEntryFactory, MicroEntryFactory>();
        services.AddSingleton<IMicroHost, MicroHost>();
        services.AddScoped<IMicroSession, MicroSession>();
        services.AddScoped<IMicroExecutor, MicroExecutor>();
        
        services.AddHostedService<HostListenerBackroungService>();

        services.Configure<ServiceAddress>(builder.Configuration.GetSection("micro"));

        return builder;
    }
}
