using Microsoft.Extensions.Logging;
using Spear.Core.Helper;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery;

namespace Spear.Core.Micro
{
    /// <summary> 服务宿主基类 </summary>
    public abstract class MicroHostBase : IMicroHost
    {
        private readonly IMicroExecutor _microExecutor;
        private readonly ILogger<MicroHostBase> _logger;

        /// <summary> 消息监听者。 </summary>
        protected IMicroListener MicroListener { get; set; }

        protected MicroHostBase(IMicroExecutor microExecutor, IMicroListener microListener, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MicroHostBase>();
            _microExecutor = microExecutor;
            MicroListener = microListener;
            MicroListener.Received += MessageListenerReceived;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            Task.Run(Stop).Wait();
        }

        /// <summary> 启动微服务 </summary>
        /// <param name="serviceAddress">主机终结点。</param>
        /// <returns>一个任务。</returns>
        public abstract Task Start(ServiceAddress serviceAddress);

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
