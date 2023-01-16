using System.Net;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Crey.Messages;

namespace Crey.Transport.TCP;

internal class ServerHandler : ChannelHandlerAdapter
{
    private readonly ILogger<ServerHandler> _logger;
    private readonly Action<IChannelHandlerContext, MessageInvoke> _readAction;

    public ServerHandler(Action<IChannelHandlerContext, MessageInvoke> readAction, ILoggerFactory loggerFactory)
    {
        _readAction = readAction;
        _logger = loggerFactory.CreateLogger<ServerHandler>();
    }

    public override Task ConnectAsync(IChannelHandlerContext context, EndPoint remoteAddress, EndPoint localAddress)
    {
        _logger.LogDebug($"ConnectAsync, client:{remoteAddress}, server:{localAddress}");
        return Task.CompletedTask;
    }

    public override Task DisconnectAsync(IChannelHandlerContext context)
    {
        _logger.LogDebug("DisconnectAsync");
        return Task.CompletedTask;
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
        _logger.LogDebug($"ChannelActive: {context.Channel.RemoteAddress}");
    }

    public override void ChannelInactive(IChannelHandlerContext context)
    {
        _logger.LogDebug($"ChannelInactive: {context.Channel.RemoteAddress}");
    }

    public override void ChannelRegistered(IChannelHandlerContext context)
    {
        _logger.LogDebug($"ChannelRegistered: {context.Channel.RemoteAddress}");
    }

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        if (message is not MessageInvoke msg)
            return;

        Task.Run(() => _readAction(context, msg));
    }


    public override void ChannelReadComplete(IChannelHandlerContext context)
    {
        context.Flush();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        context.CloseAsync();
        _logger.LogWarning(exception, $"Error：{context.Channel.RemoteAddress}");
    }


}
