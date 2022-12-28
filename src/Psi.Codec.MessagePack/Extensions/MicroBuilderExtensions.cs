using Microsoft.Extensions.DependencyInjection;
using Psi.Builder.Abstractions;
using Psi.Codec.MessagePack;
using Psi.Message.Abstractions;

namespace Psi.Codec.MessagePack.Extensions;

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
