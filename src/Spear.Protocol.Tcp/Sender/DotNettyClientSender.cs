using DotNetty.Transport.Channels;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;

namespace Spear.Protocol.Tcp.Sender;

public class DotNettyClientSender : DotNettyMessageSender, IMessageSender, IDisposable
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

    public async Task Send(DMessage message, bool flush = true)
    {
        var buffer = await GetByteBuffer(message);
        if (flush)
            await _channel.WriteAndFlushAsync(buffer);
        else
            await _channel.WriteAsync(buffer);
    }
}
