using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Crey.Helper;
using Crey.Message;

namespace Crey.Protocol.Tcp;

internal class ClientHandler : ChannelHandlerAdapter
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

    public override void ChannelInactive(IChannelHandlerContext context)
    {
        _removeAction?.Invoke(context.Channel);
    }

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        _logger.LogDebug(JsonHelper.ToJson(message));

        if (message is not MessageResult msg)
            return;

        _readAction?.Invoke(context, msg);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        _logger.LogError(exception, $"Error：{context.Channel.RemoteAddress}");
    }
}
