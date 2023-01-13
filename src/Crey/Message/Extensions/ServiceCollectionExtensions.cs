using Crey.Message;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Codec;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MessagePack serializer
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMessageCodec(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSerializer, MessageSerializer>();
        services.AddSingleton<IMessageCodec, MessageCodec>();

        return services;
    }
}
