using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Crey.Helper;
using Crey.Message;
using Crey.ServiceDiscovery.Models;

namespace Crey.Micro;

public abstract class MicroHostBase : IMicroHost
{
    private readonly ILogger<MicroHostBase> _logger;
    private readonly IServiceProvider _serviceProvider;

    protected IMicroListener MicroListener { get; set; }

    protected MicroHostBase(IServiceProvider serviceProvider, IMicroListener microListener, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MicroHostBase>();
        _serviceProvider = serviceProvider;

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

        // execute each request in it's own service scope
        await using var scope = _serviceProvider.CreateAsyncScope();

        var microExecutor = scope.ServiceProvider.GetRequiredService<IMicroExecutor>();

        await microExecutor.Execute(sender, invokeMessage);
    }
}
