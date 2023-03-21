using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Crey.Helpers;
using Crey.Messages;

namespace Crey.Transport.TCP;

internal class DotNettyClientHandler : ChannelHandlerAdapter
{
    private readonly Action<IChannel> _removeAction;
    private readonly Action<IChannelHandlerContext, Messages.Message> _readAction;
    private readonly ILogger<DotNettyClientHandler> _logger;

    public DotNettyClientHandler(Action<IChannelHandlerContext, Messages.Message> readAction, Action<IChannel> removeAction, ILoggerFactory loggerFactory)
    {
        _removeAction = removeAction;
        _readAction = readAction;
        _logger = loggerFactory.CreateLogger<DotNettyClientHandler>();
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
