using Crey.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crey.Transport;

public abstract class TransportListener : MessageListener, ITransportListener
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TransportListener> _logger;

    protected TransportListener(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = loggerFactory.CreateLogger<TransportListener>();

        Received += MessageListenerReceived;
    }

    public abstract Task Start(ServiceAddress serviceAddress);

    public abstract Task Stop();

    private async Task MessageListenerReceived(IMessageSender sender, Messages.Message message)
    {
        if (message is not MessageInvoke invokeMessage)
            return;

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug($"receive:{JsonHelper.ToJson(message)}");

        // execute each request in it's own service scope
        await using var scope = _serviceProvider.CreateAsyncScope();

        var microExecutor = scope.ServiceProvider.GetRequiredService<IServiceMethodExecutor>();

        await microExecutor.Execute(sender, invokeMessage);
    }
}
