using Microsoft.Extensions.DependencyInjection;

namespace Crey.Codec.MessagePack;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MessagePack serializer
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMessagePackCodec(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSerializer, MessagePackMessageSerializer>();
        services.AddSingleton<IMessageCodec, MessagePackCodec>();

        return services;
    }
}
