﻿using DotNetty.Transport.Channels;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;

namespace Spear.Protocol.Tcp.Sender
{
    /// <summary>
    /// 基于DotNetty客户端的消息发送者。
    /// </summary>
    public class DotNettyClientSender : DotNettyMessageSender, IMessageSender, IDisposable
    {
        private readonly IChannel _channel;

        public DotNettyClientSender(IMessageEncoder messageEncoder, IChannel channel)
            : base(messageEncoder)
        {
            _channel = channel;
        }

        public void Dispose()
        {
            Task.Run(_channel.DisconnectAsync).Wait();
        }

        /// <summary> 发送消息 </summary>
        /// <param name="message">消息内容</param>
        /// <param name="flush"></param>
        /// <returns>一个任务。</returns>
        public async Task Send(DMessage message, bool flush = true)
        {
            var buffer = await GetByteBuffer(message);
            if (flush)
            {
                await _channel.WriteAndFlushAsync(buffer);
            }
            else
                await _channel.WriteAsync(buffer);
        }
    }
}
