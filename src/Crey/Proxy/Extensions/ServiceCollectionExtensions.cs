using Microsoft.Extensions.DependencyInjection;

namespace Crey.Proxy;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Proxy services to <see cref="IServiceCollection"/>
    /// </summary>
    public static IServiceCollection AddProxyServices(this IServiceCollection services)
    {
        services.AddSingleton<ProxyInterceptor>();
        services.AddSingleton<IProxyFactory, ProxyFactory>();
        services.AddSingleton<IProxyProvider, ProxyProvider>();
        services.AddSingleton<IProxyExecutor, ProxyExecutor>();

        return services;
    }
}
