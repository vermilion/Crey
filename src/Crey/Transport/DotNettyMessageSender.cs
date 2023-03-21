using DotNetty.Buffers;

namespace Crey.Transport;

public abstract class DotNettyMessageSender
{
    private readonly IMessageCodec _codec;

    protected DotNettyMessageSender(IMessageCodec codec)
    {
        _codec = codec;
    }

    protected async Task<IByteBuffer> GetByteBuffer(Message message)
    {
        var data = await _codec.EncodeAsync(message);
        return Unpooled.WrappedBuffer(data);
    }
}
