using Crey.BackgroundServices;
using Crey.Builder;
using Crey.Proxy;
using Crey.Discovery;
using Crey.Session;
using Microsoft.Extensions.DependencyInjection;
using Crey.Codec;

namespace Crey.Micro;

public static class ServiceCollectionExtensions
{
    public static IMicroBuilder AddMicroClient(this IMicroBuilder builder, Action<IMicroClientBuilder> builderAction = null)
    {
        var services = builder.Services;

        builderAction?.Invoke(builder);

        services.AddBase();

        // proxy services
        services.AddProxyServices();

        return builder;
    }

    public static IMicroBuilder AddMicroService(this IMicroBuilder builder, Action<IMicroServerBuilder> builderAction)
    {
        var services = builder.Services;

        builderAction.Invoke(builder);

        services.AddBase();
        services.AddSingleton<IMicroHost, MicroHost>();
        services.AddScoped<IMicroExecutor, MicroExecutor>();
        services.AddSingleton<IMicroEntryFactory, MicroEntryFactory>();

        services.AddScoped<ICallContextAccessor, CallContextAccessor>();

        services.AddHostedService<HostListenerBackroungService>();

        services.Configure<ServiceAddress>(builder.ConfigurationSection.GetSection("service"));

        return builder;
    }

    private static void AddBase(this IServiceCollection services)
    {
        services.AddSingleton(services);

        services.AddMessageCodec();
    }
}
