using Crey.Extensions;

namespace Crey.Codec;

public static class MessageCodecExtensions
{
    public static T Decode<T>(this IMessageCodec codec, byte[] data)
    {
        var obj = codec.DecodeAsync(data, typeof(T)).ConfigureAwait(false).GetAwaiter().GetResult();
        return obj.CastTo<T>();
    }
}
