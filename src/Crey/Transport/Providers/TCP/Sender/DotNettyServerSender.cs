using DotNetty.Transport.Channels;

namespace Crey.Transport.TCP;

internal class DotNettyServerSender : DotNettyMessageSender, IMessageSender
{
    private readonly IChannelHandlerContext _context;

    public DotNettyServerSender(IMessageCodec codec, IChannelHandlerContext context)
        : base(codec)
    {
        _context = context;
    }

    public async Task Send(Message message, bool flush = true)
    {
        var buffer = await GetByteBuffer(message);
        if (flush)
            await _context.WriteAndFlushAsync(buffer);
        else
            await _context.WriteAsync(buffer);
    }
}
