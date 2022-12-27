using DotNetty.Buffers;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;

namespace Spear.Protocol.Tcp.Sender;

public abstract class DotNettyMessageSender
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
