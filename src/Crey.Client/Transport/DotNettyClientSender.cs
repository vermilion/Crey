using Crey.Codec;
using Crey.Messages;
using DotNetty.Transport.Channels;

namespace Crey.Transport.TCP;

internal class DotNettyClientSender : DotNettyMessageSender, IMessageSender, IDisposable
{
    private readonly IChannel _channel;

    public DotNettyClientSender(IMessageCodec codec, IChannel channel)
        : base(codec)
    {
        _channel = channel;
    }

    public void Dispose()
    {
        Task.Run(_channel.DisconnectAsync).Wait();
    }

    public async Task Send(Message message, bool flush = true)
    {
        var buffer = await GetByteBuffer(message);
        if (flush)
            await _channel.WriteAndFlushAsync(buffer);
        else
            await _channel.WriteAsync(buffer);
    }
}
