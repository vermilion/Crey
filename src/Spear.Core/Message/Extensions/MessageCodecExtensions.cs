﻿using Spear.Core.Extensions;
using Spear.Core.Message.Abstractions;

namespace Spear.Core.Message.Extensions;

public static class MessageCodecExtensions
{
    public static T Decode<T>(this IMessageDecoder decoder, byte[] data)
    {
        var obj = decoder.DecodeAsync(data, typeof(T)).ConfigureAwait(false).GetAwaiter().GetResult();
        return obj.CastTo<T>();
    }
}
