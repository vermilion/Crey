using Crey.Micro;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Protocol;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds DotNetty TCP transport protocol
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddTcpProtocol(this IServiceCollection services)
    {
        services.AddSingleton<IMicroListener, DotNettyMicroListener>();
        services.AddSingleton<IMicroClientFactory, DotNettyClientFactory>();

        return services;
    }
}
