﻿using System.Text.Json;
using Microsoft.Extensions.Logging;
using Spear.Core.Helper;
using Spear.Core.Message;
using Spear.Core.Message.Models;
using Spear.Core.Micro.Services;

namespace Spear.Core.Micro.Implementation
{
    /// <summary> 服务宿主基类 </summary>
    public abstract class DMicroHost : IMicroHost
    {
        private readonly IMicroExecutor _microExecutor;
        private readonly ILogger<DMicroHost> _logger;

        /// <summary> 消息监听者。 </summary>
        protected IMicroListener MicroListener { get; set; }

        protected DMicroHost(IMicroExecutor microExecutor, IMicroListener microListener, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DMicroHost>();
            _microExecutor = microExecutor;
            MicroListener = microListener;
            MicroListener.Received += MessageListenerReceived;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            Task.Run(async () => await Stop()).Wait();
        }

        /// <summary> 启动微服务 </summary>
        /// <param name="serviceAddress">主机终结点。</param>
        /// <returns>一个任务。</returns>
        public abstract Task Start(ServiceAddress serviceAddress);

        public Task Start(string host, int port)
        {
            return Start(new ServiceAddress(host, port));
        }

        public abstract Task Stop();

        private async Task MessageListenerReceived(IMessageSender sender, DMessage message)
        {
            if (message is not InvokeMessage invokeMessage)
                return;
            
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"receive:{JsonHelper.ToJson(message)}");

            await _microExecutor.Execute(sender, invokeMessage);
        }
    }
}
