namespace Crey.Transport;

public abstract class TransportListener : MessageListener, ITransportListener
{
    private readonly IServiceMethodExecutor _methodExecutor;
    private readonly ILogger<TransportListener> _logger;

    protected TransportListener(IServiceMethodExecutor methodExecutor, ILoggerFactory loggerFactory)
    {
        _methodExecutor = methodExecutor;
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

        await _methodExecutor.Execute(sender, invokeMessage);
    }
}
