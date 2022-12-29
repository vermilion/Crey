using Crey.Extensions;
using Crey.Message.Abstractions;

namespace Crey.Message.Extensions;

public static class MessageCodecExtensions
{
    public static T Decode<T>(this IMessageDecoder decoder, byte[] data)
    {
        var obj = decoder.DecodeAsync(data, typeof(T)).ConfigureAwait(false).GetAwaiter().GetResult();
        return obj.CastTo<T>();
    }
}
