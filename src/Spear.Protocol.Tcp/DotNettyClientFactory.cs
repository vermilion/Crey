using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Spear.Core.Message;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;
using Spear.Core.Micro;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery;
using Spear.Core.ServiceDiscovery.Extensions;
using Spear.Protocol.Tcp.Adapter;
using Spear.Protocol.Tcp.Sender;

namespace Spear.Protocol.Tcp
{
    public class DotNettyClientFactory : MicroClientFactory
    {
        private static readonly AttributeKey<ServiceAddress> ServiceAddressKey =
            AttributeKey<ServiceAddress>.ValueOf(typeof(DotNettyClientFactory), nameof(ServiceAddress));

        private static readonly AttributeKey<IMessageSender> SenderKey =
            AttributeKey<IMessageSender>.ValueOf(typeof(DotNettyClientFactory), nameof(IMessageSender));
        private static readonly AttributeKey<IMessageListener> ListenerKey =
            AttributeKey<IMessageListener>.ValueOf(typeof(DotNettyClientFactory), nameof(IMessageListener));
        private readonly IMessageCodec _codec;

        public DotNettyClientFactory(
            ILoggerFactory loggerFactory,
            IServiceProvider provider,
            IMessageCodec codec,
            IMicroExecutor executor)
            : base(loggerFactory, provider, executor)
        {
            _codec = codec;
        }

        /// <inheritdoc />
        /// <summary> 创建客户端 </summary>
        /// <param name="serviceAddress">终结点。</param>
        /// <returns>传输客户端实例。</returns>
        protected override async Task<IMicroClient> Create(ServiceAddress serviceAddress)
        {
            var bootstrap = CreateBootstrap();
            var channel = await bootstrap.ConnectAsync(serviceAddress.ToEndpoint());
            var listener = new MessageListener();
            var sender = new DotNettyClientSender(_codec, channel);

            channel.GetAttribute(ListenerKey).Set(listener);
            channel.GetAttribute(SenderKey).Set(sender);
            channel.GetAttribute(ServiceAddressKey).Set(serviceAddress);

            return new MicroClient(sender, listener, MicroExecutor, LoggerFactory);
        }

        private Bootstrap CreateBootstrap()
        {
            var bootstrap = GetBootstrap();

            bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(c =>
            {
                var pipeline = c.Pipeline;

                pipeline.AddLast(new LengthFieldPrepender(4));
                pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                pipeline.AddLast(new MicroMessageHandler<MessageResult>(_codec));
                pipeline.AddLast(new ClientHandler((context, message) =>
                {
                    var messageListener = context.Channel.GetAttribute(ListenerKey).Get();
                    var messageSender = context.Channel.GetAttribute(SenderKey).Get();
                    messageListener.OnReceived(messageSender, message);
                }, channel =>
                {
                    var k = channel.GetAttribute(ServiceAddressKey).Get();
                    Remove(k);
                }, LoggerFactory));
            }));

            return bootstrap;
        }

        private static Bootstrap GetBootstrap()
        {
            IEventLoopGroup group;
            var bootstrap = new Bootstrap();

            group = new MultithreadEventLoopGroup();
            bootstrap.Channel<TcpServerSocketChannel>();

            bootstrap
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                .Group(group);

            return bootstrap;
        }
    }
}
