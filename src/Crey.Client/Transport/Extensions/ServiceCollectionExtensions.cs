using Crey.ClientSide;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Transport.TCP;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds DotNetty TCP transport protocol
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddClientTcpProtocolPart(this IServiceCollection services)
    {
        services.AddSingleton<IClientFactory, DotNettyClientFactory>();

        return services;
    }
}
