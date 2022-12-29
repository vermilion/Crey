using DotNetty.Transport.Channels;
using Crey.Message.Abstractions;
using Crey.Message.Models;

namespace Crey.Protocol.Tcp.Sender;

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
