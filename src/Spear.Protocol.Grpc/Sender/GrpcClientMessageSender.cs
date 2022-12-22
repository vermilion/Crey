using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Spear.Core.Message;
using Spear.Core.Message.Models;
using Spear.Core.Micro.Services;

namespace Spear.Protocol.Grpc.Sender
{
    public class GrpcClientMessageSender : IMessageSender
    {
        private readonly ServiceAddress _address;
        private readonly IMessageCodec _messageCodec;
        private readonly IMessageListener _listener;

        public GrpcClientMessageSender(ServiceAddress address, IMessageCodec messageCodec, IMessageListener listener)
        {
            _address = address;
            _messageCodec = messageCodec;
            _listener = listener;
        }

        public async Task Send(DMessage message, bool flush = true)
        {
            if (message is not InvokeMessage invokeMessage)
                return;

            var channel = new Channel(_address.Host, _address.Port, ChannelCredentials.Insecure);
            var invoker = new DefaultCallInvoker(channel);

            var method = MethodDefinitionGenerator
                .CreateMethodDefinition<InvokeMessage, MessageResult>(MethodType.Unary, "Micro", "Unary", _messageCodec);

            var callOptions = new CallOptions(cancellationToken: CancellationToken.None);

            using var call = invoker.AsyncUnaryCall(method, null, callOptions, invokeMessage);

            var result = await call.ResponseAsync.ConfigureAwait(false);

            await _listener.OnReceived(this, result);
        }
    }
}
