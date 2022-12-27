using Microsoft.Extensions.DependencyInjection;
using Spear.Core.BackgroundServices;
using Spear.Core.Builder.Abstractions;
using Spear.Core.Micro;
using Spear.Core.Micro.Abstractions;
using Spear.Core.Proxy;
using Spear.Core.ServiceDiscovery.Models;
using Spear.Core.ServiceDiscovery.StaticRouter.Options;
using Spear.ProxyGenerator.Extensions;

namespace Spear.Core.Builder.Extensions;

public static class ServiceCollectionExtensions
{
    public static IMicroBuilder AddMicroClient(this IMicroBuilder builder, Action<IMicroClientBuilder> builderAction)
    {
        builder.Services.AddProxy<ClientProxy>();
        builderAction.Invoke(builder);

        return builder;
    }

    public static IMicroBuilder AddMicroService(this IMicroBuilder builder, Action<IMicroServerBuilder> builderAction, Action<StaticRouterOptions> configAction = null)
    {
        var services = builder.Services;

        services.AddSingleton(services);
        services.AddSingleton<IMicroEntryFactory, MicroEntryFactory>();
        services.AddSingleton<IMicroExecutor, MicroExecutor>();
        services.AddSingleton<IMicroHost, MicroHost>();

        services.AddHostedService<HostListenerBackroungService>();

        builderAction.Invoke(builder);

        services.Configure<ServiceAddress>(builder.Configuration.GetSection("micro"));

        return builder;
    }
}
