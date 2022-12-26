﻿using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Spear.Core.Message;
using Spear.Core.Message.Models;

namespace Spear.Protocol.Tcp.Adapter
{
    public class MicroMessageHandler<T> : ChannelHandlerAdapter
        where T : DMessage
    {
        private readonly IMessageCodec _codec;

        public MicroMessageHandler(IMessageCodec codec)
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
}
