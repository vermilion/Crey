using Microsoft.Extensions.DependencyInjection;
using Spear.ProxyGenerator.Abstractions;
using Spear.ProxyGenerator.Proxy;

namespace Spear.ProxyGenerator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxy<T>(this IServiceCollection services)
        where T : class, IProxyProvider
    {
        services.AddSingleton<AsyncProxyGenerator>();
        services.AddSingleton<IResolver, ProxyResolver>();
        services.AddSingleton<IProxyProvider, T>();
        services.AddSingleton<IProxyFactory, ProxyFactory>();
        return services;
    }
}
