using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Health.V1;
using Grpc.HealthCheck;
using Microsoft.Extensions.Logging;
using Spear.Core.Exceptions;
using Spear.Core.Message;
using Spear.Core.Message.Models;
using Spear.Core.Micro.Implementation;
using Spear.Core.Micro.Services;
using Spear.Protocol.Grpc.Sender;

namespace Spear.Protocol.Grpc
{
    public class GrpcMicroListener : MicroListener
    {
        private Server _grpcServer;

        private readonly ILogger<GrpcMicroListener> _logger;
        private readonly IMessageCodec _messageCodec;

        public GrpcMicroListener(
            ILoggerFactory loggerFactory,
            IMessageCodec messageCodec)
        {
            _messageCodec = messageCodec;
            _logger = loggerFactory.CreateLogger<GrpcMicroListener>();
        }

        public override Task Start(ServiceAddress serviceAddress)
        {
            var builder = ServerServiceDefinition.CreateBuilder();

            var method = MethodDefinitionGenerator
                .CreateMethodDefinition<InvokeMessage, MessageResult>(MethodType.Unary, "Micro", "Unary", _messageCodec);

            var handler = new UnaryServerMethod<InvokeMessage, MessageResult>(async (invokeMessage, context) =>
            {
                var completion = new TaskCompletionSource<MessageResult>();

                var sender = new GrpcServerMessageSender(completion);

                try
                {
                    await OnReceived(sender, invokeMessage, context);
                }
                catch (Exception ex)
                {
                    var result = new MessageResult();
                    if (ex is SpearException busi)
                    {
                        result.Code = busi.Code;
                        result.Message = busi.Message;
                    }
                    else
                    {
                        _logger.LogError(ex, ex.Message);
                        result.Code = (int)HttpStatusCode.InternalServerError;
                        result.Message = ex.Message;
                    }

                    return result;
                }

                return await completion.Task;
            });

            builder.AddMethod(method, handler);

            var healthService = new HealthServiceImpl();

            _grpcServer = new Server
            {
                Ports =
                {
                    new ServerPort(serviceAddress.Service, serviceAddress.Port, ServerCredentials.Insecure)
                },
                Services =
                {
                    Health.BindService(healthService),
                    builder.Build()
                }
            };

            _logger.LogInformation($"GRPC Service Start At:{serviceAddress}");
            _grpcServer.Start();

            return Task.CompletedTask;
        }

        private async Task OnReceived(IMessageSender sender, InvokeMessage invokeMessage, ServerCallContext context)
        {
            invokeMessage.Headers ??= new Dictionary<string, string>();
            foreach (var header in context.RequestHeaders)
            {
                invokeMessage.Headers[header.Key] = header.Value;
            }

            await OnReceived(sender, invokeMessage);
        }

        public override Task Stop()
        {
            return _grpcServer?.ShutdownAsync();
        }
    }
}
