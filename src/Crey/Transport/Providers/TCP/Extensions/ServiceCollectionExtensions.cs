using Microsoft.Extensions.DependencyInjection;

namespace Crey.Transport.TCP;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds DotNetty TCP transport protocol
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddTcpProtocol(this IServiceCollection services)
    {
        services.AddSingleton<ITransportListener, DotNettyTransportListener>();
        services.AddSingleton<IClientFactory, DotNettyClientFactory>();

        return services;
    }
}
