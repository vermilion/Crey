﻿using Crey.Codec.Extensions;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace Crey.Transport;

public class DotNettyChannelHandlerAdapter<T> : ChannelHandlerAdapter
    where T : Message
{
    private readonly IMessageCodec _codec;

    public DotNettyChannelHandlerAdapter(IMessageCodec codec)
    {
        _codec = codec;
    }

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        var buffer = (IByteBuffer)message;
        var data = new byte[buffer.ReadableBytes];
        buffer.ReadBytes(data);

        var microMessage = _codec.Decode<T>(data);
        context.FireChannelRead(microMessage);
        ReferenceCountUtil.Release(buffer);
    }
}
