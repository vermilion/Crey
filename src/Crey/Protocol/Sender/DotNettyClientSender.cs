using DotNetty.Transport.Channels;
using Crey.Message;

namespace Crey.Protocol;

internal class DotNettyClientSender : DotNettyMessageSender, IMessageSender, IDisposable
{
    private readonly IChannel _channel;

    public DotNettyClientSender(IMessageEncoder messageEncoder, IChannel channel)
        : base(messageEncoder)
    {
        _channel = channel;
    }

    public void Dispose()
    {
        Task.Run(_channel.DisconnectAsync).Wait();
    }

    public async Task Send(Message.Message message, bool flush = true)
    {
        var buffer = await GetByteBuffer(message);
        if (flush)
            await _channel.WriteAndFlushAsync(buffer);
        else
            await _channel.WriteAsync(buffer);
    }
}
