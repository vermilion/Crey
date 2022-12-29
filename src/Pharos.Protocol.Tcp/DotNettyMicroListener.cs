using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Psi.Message.Abstractions;
using Psi.Message.Models;
using Psi.ServiceDiscovery.Extensions;
using Psi.ServiceDiscovery.Models;
using Psi.Protocol.Tcp.Adapter;
using Psi.Protocol.Tcp.Sender;
using Psi.Micro;

namespace Psi.Protocol.Tcp;

public class DotNettyMicroListener : MicroListener, IDisposable
{
    private IChannel? _channel;
    private readonly ILogger<DotNettyMicroListener> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMessageCodec _messageCodec;

    public DotNettyMicroListener(ILoggerFactory loggerFactory, IMessageCodec messageCodec)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<DotNettyMicroListener>();
        _messageCodec = messageCodec;
    }

    public override async Task Start(ServiceAddress serviceAddress)
    {
        _logger.LogDebug($"Starting TCP listener：{serviceAddress}");

        var bossGroup = new MultithreadEventLoopGroup(1);
        var workerGroup = new MultithreadEventLoopGroup();
        var bootstrap = new ServerBootstrap();
        bootstrap
            .Channel<TcpServerSocketChannel>()
            .Option(ChannelOption.SoBacklog, 8192)
            .ChildOption(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
            .Group(bossGroup, workerGroup)
            .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                var pipeline = channel.Pipeline;
                pipeline.AddLast(new LengthFieldPrepender(4));
                pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                pipeline.AddLast(new MicroMessageHandler<InvokeMessage>(_messageCodec));
                pipeline.AddLast(new ServerHandler(async (context, message) =>
                {
                    var sender = new DotNettyServerSender(_messageCodec, context);
                    await OnReceived(sender, message);
                }, _loggerFactory));
            }));

        try
        {
            _channel = await bootstrap.BindAsync(serviceAddress.ToEndpoint());
            _logger.LogInformation($"TCP listener started at：{serviceAddress}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to start TCP listener at：{serviceAddress}");
            throw;
        }
    }

    public override Task Stop()
    {
        return Task.Run(() =>
        {
            Dispose();
            //await _channel.EventLoop.ShutdownGracefullyAsync();
            //await _channel.CloseAsync();
        });
    }

    public void Dispose()
    {
        _channel?.DisconnectAsync().GetAwaiter().GetResult();
    }
}
