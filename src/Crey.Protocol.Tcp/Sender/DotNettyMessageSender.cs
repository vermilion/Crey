using DotNetty.Buffers;
using Crey.Message;

namespace Crey.Protocol.Tcp;

internal abstract class DotNettyMessageSender
{
    private readonly IMessageEncoder _messageEncoder;

    protected DotNettyMessageSender(IMessageEncoder messageEncoder)
    {
        _messageEncoder = messageEncoder;
    }

    protected async Task<IByteBuffer> GetByteBuffer(Message.Message message)
    {
        var data = await _messageEncoder.EncodeAsync(message);
        return Unpooled.WrappedBuffer(data);
    }
}
