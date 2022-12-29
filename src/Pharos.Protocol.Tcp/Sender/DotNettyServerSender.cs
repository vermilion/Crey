using DotNetty.Transport.Channels;
using Psi.Message.Abstractions;
using Psi.Message.Models;

namespace Psi.Protocol.Tcp.Sender;

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
