using System.Net;
using Crey.Service;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Crey.Transport.TCP;

internal class DotNettyTransportListener : MessageListener, ITransportListener, IDisposable
{
    private IChannel? _channel;
    private readonly IServiceMethodExecutor _methodExecutor;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMessageCodec _messageCodec;
    private readonly ILogger<DotNettyTransportListener> _logger;

    public DotNettyTransportListener(
        IServiceMethodExecutor methodExecutor,
        ILoggerFactory loggerFactory,
        IMessageCodec messageCodec)
    {
        _methodExecutor = methodExecutor;
        _loggerFactory = loggerFactory;
        _messageCodec = messageCodec;
        _logger = loggerFactory.CreateLogger<DotNettyTransportListener>();

        Received += MessageListenerReceived;
    }

    public async Task Start(ServiceAddress serviceAddress)
    {
        var ipEndpoint = new IPEndPoint(IPAddress.Any, serviceAddress.Port);

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug($"Starting TCP listener at：{ipEndpoint}");

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
                pipeline.AddLast(new DotNettyChannelHandlerAdapter<MessageInvoke>(_messageCodec));
                pipeline.AddLast(new DotNettyServerHandler(async (context, message) =>
                {
                    var sender = new DotNettyServerSender(_messageCodec, context);
                    await OnReceived(sender, message);
                }, _loggerFactory));
            }));

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

    public Task Stop()
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

    private async Task MessageListenerReceived(IMessageSender sender, Message message)
    {
        if (message is not MessageInvoke invokeMessage)
            return;

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug($"receive:{JsonHelper.ToJson(message)}");

        await _methodExecutor.Execute(sender, invokeMessage);
    }
}
