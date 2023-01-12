using Crey.Builder;
using Crey.Message;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Codec;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MessagePack serializer
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <returns>Builder instance</returns>
    public static IServiceCollection AddMessageCodec(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSerializer, MessageSerializer>();
        services.AddSingleton<IMessageCodec, MessageCodec>();

        return services;
    }
}
