using Microsoft.Extensions.DependencyInjection;
using Crey.Builder;
using Crey.Message;

namespace Crey.Codec.MessagePack;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds MessagePack serializer as default
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddMessagePackCodec(this IMicroBuilder builder)
    {
        builder.Services.AddSingleton<IMessageSerializer, MessagePackMessageSerializer>();
        builder.Services.AddSingleton<IMessageCodec, MessagePackCodec>();

        return builder;
    }
}
