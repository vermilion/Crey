using DotNetty.Transport.Channels;
using Crey.Message;

namespace Crey.Protocol.Tcp;

internal class DotNettyServerSender : DotNettyMessageSender, IMessageSender
{
    private readonly IChannelHandlerContext _context;

    public DotNettyServerSender(IMessageEncoder messageEncoder, IChannelHandlerContext context)
        : base(messageEncoder)
    {
        _context = context;
    }

    public async Task Send(Message.Message message, bool flush = true)
    {
        var buffer = await GetByteBuffer(message);
        if (flush)
            await _context.WriteAndFlushAsync(buffer);
        else
            await _context.WriteAsync(buffer);
    }
}
