using Microsoft.Extensions.Logging;
using Spear.Core.Helper;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.Micro;

public abstract class MicroHostBase : IMicroHost
{
    private readonly IMicroExecutor _microExecutor;
    private readonly ILogger<MicroHostBase> _logger;

    protected IMicroListener MicroListener { get; set; }

    protected MicroHostBase(IMicroExecutor microExecutor, IMicroListener microListener, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MicroHostBase>();
        _microExecutor = microExecutor;

        MicroListener = microListener;
        MicroListener.Received += MessageListenerReceived;
    }

    public virtual void Dispose()
    {
        Task.Run(Stop).Wait();
    }

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
