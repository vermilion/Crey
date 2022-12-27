using DotNetty.Transport.Channels;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;

namespace Spear.Protocol.Tcp.Sender;

public class DotNettyServerSender : DotNettyMessageSender, IMessageSender
{
    private readonly IChannelHandlerContext _context;

    public DotNettyServerSender(IMessageEncoder messageEncoder, IChannelHandlerContext context)
        : base(messageEncoder)
    {
        _context = context;
    }

    public async Task Send(DMessage message, bool flush = true)
    {
        var buffer = await GetByteBuffer(message);
        if (flush)
            await _context.WriteAndFlushAsync(buffer);
        else
            await _context.WriteAsync(buffer);
    }
}
