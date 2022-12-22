using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Spear.Core.Message;
using Spear.Core.Message.Implementation;
using Spear.Core.Micro;
using Spear.Core.Micro.Implementation;
using Spear.Core.Micro.Services;
using Spear.Protocol.Grpc.Sender;

namespace Spear.Protocol.Grpc
{
    public class GrpcClientFactory : DMicroClientFactory
    {
        private readonly IMessageCodec _messageCodec;

        public GrpcClientFactory(ILoggerFactory loggerFactory, IServiceProvider provider, IMicroExecutor microExecutor, IMessageCodec messageCodec)
            : base(loggerFactory, provider, microExecutor)
        {
            _messageCodec = messageCodec;
        }

        protected override Task<IMicroClient> Create(ServiceAddress address)
        {
            Logger.LogDebug($"创建客户端：{address}创建客户端。");

            var listener = new MessageListener();
            var sender = new GrpcClientMessageSender(address, _messageCodec, listener);

            IMicroClient client = new MicroClient(sender, listener, MicroExecutor, LoggerFactory);
            return Task.FromResult(client);
        }
    }
}
