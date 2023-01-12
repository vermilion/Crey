using Microsoft.Extensions.DependencyInjection;

namespace Crey.Proxy;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Proxy services to <see cref="IServiceCollection"/>
    /// </summary>
    public static IServiceCollection AddProxyServices(this IServiceCollection services)
    {
        services.AddSingleton<ClientProxyInterceptor>();
        services.AddSingleton<IProxyFactory, ProxyFactory>();
        services.AddSingleton<IProxyProvider, ClientProxy>();

        return services;
    }
}
