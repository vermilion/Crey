﻿using System.Text.Json;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Spear.Core.Helper;
using Spear.Core.Message.Models;

namespace Spear.Protocol.Tcp.Adapter
{
    /// <summary> 客户端处理器 </summary>
    public class ClientHandler : ChannelHandlerAdapter
    {
        private readonly Action<IChannel> _removeAction;
        private readonly Action<IChannelHandlerContext, DMessage> _readAction;
        private readonly ILogger<ClientHandler> _logger;

        public ClientHandler(Action<IChannelHandlerContext, DMessage> readAction, Action<IChannel> removeAction, ILoggerFactory loggerFactory)
        {
            _removeAction = removeAction;
            _readAction = readAction;
            _logger = loggerFactory.CreateLogger<ClientHandler>();
        }

        /// <summary> 连接断开 </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _removeAction?.Invoke(context.Channel);
        }

        /// <summary> 收到数据 </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            _logger.LogDebug(JsonHelper.ToJson(message));

            if (message is not MessageResult msg)
                return;

            _readAction?.Invoke(context, msg);
        }

        /// <summary> 发生异常 </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.LogError(exception, $"与服务器：{context.Channel.RemoteAddress}通信时发送了错误。");
        }
    }
}
