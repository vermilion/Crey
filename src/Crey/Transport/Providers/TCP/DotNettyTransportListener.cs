using System.Net;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;

namespace Crey.Transport.TCP;

internal class DotNettyTransportListener : TransportListener, IDisposable
{
    private IChannel? _channel;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<DotNettyTransportListener> _logger;
    private readonly IMessageCodec _messageCodec;

    public DotNettyTransportListener(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IMessageCodec messageCodec)
        : base(serviceProvider, loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<DotNettyTransportListener>();
        _messageCodec = messageCodec;
    }

    public override async Task Start(ServiceAddress serviceAddress)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
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
                pipeline.AddLast(new MicroMessageHandler<MessageInvoke>(_messageCodec));
                pipeline.AddLast(new ServerHandler(async (context, message) =>
                {
                    var sender = new DotNettyServerSender(_messageCodec, context);
                    await OnReceived(sender, message);
                }, _loggerFactory));
            }));

        var ipEndpoint = new IPEndPoint(IPAddress.Any, serviceAddress.Port);

        try
        {
            _channel = await bootstrap.BindAsync(ipEndpoint);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"TCP listener started at： {ipEndpoint}");
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, $"Failed to start TCP listener at： {ipEndpoint}");

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
