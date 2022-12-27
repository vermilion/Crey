using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Builder;
using Spear.Core.Message.Abstractions;

namespace Spear.Codec.MessagePack
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> 使用Json编解码器。 </summary>
        /// <param name="builder">服务构建者。</param>
        /// <returns>服务构建者。</returns>
        public static IMicroBuilder AddMessagePackCodec(this IMicroBuilder builder)
        {
            builder.Services.AddSingleton<IMessageSerializer, MessagePackMessageSerializer>();
            builder.Services.AddSingleton<IMessageCodec, MessagePackCodec>(provider =>
            {
                var serializer = provider.GetRequiredService<IMessageSerializer>();
                return new MessagePackCodec(serializer);
            });

            return builder;
        }
    }
}
