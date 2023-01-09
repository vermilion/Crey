using DotNetty.Buffers;
using Crey.Message.Abstractions;
using Crey.Message.Models;

namespace Crey.Protocol.Tcp.Sender;

internal abstract class DotNettyMessageSender
{
    private readonly IMessageEncoder _messageEncoder;

    protected DotNettyMessageSender(IMessageEncoder messageEncoder)
    {
        _messageEncoder = messageEncoder;
    }

    protected async Task<IByteBuffer> GetByteBuffer(DMessage message)
    {
        var data = await _messageEncoder.EncodeAsync(message);
        return Unpooled.WrappedBuffer(data);
    }
}
