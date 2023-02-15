using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crey.Service;

public class ServiceMethodExecutor : IServiceMethodExecutor
{
    private readonly ILogger<ServiceMethodExecutor> _logger;
    private readonly IServiceEntryFactory _entryFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IExceptionConverter _exceptionTransformer;

    public ServiceMethodExecutor(
        ILogger<ServiceMethodExecutor> logger,
        IExceptionConverter exceptionTransformer,
        IServiceEntryFactory entryFactory,
        IServiceScopeFactory scopeFactory)
    {
        _exceptionTransformer = exceptionTransformer;
        _entryFactory = entryFactory;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IMessageSender sender, MessageInvoke message)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(JsonHelper.ToJson(message));

        var entry = _entryFactory.Find(message.ServiceId);
        if (entry == null)
        {
            await SendResult(sender, message.Id, new MessageResult(ExceptionConstants.ServiceNotFound));
            return;
        }

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation($"Execute, InvokeType: {message.Context.Type}");

        switch (message.Context.Type)
        {
            case MessageInvokeContextType.OneWay:
                {
                    await ExecuteOneWay(sender, entry, message);
                    break;
                }
            case MessageInvokeContextType.None:
            default:
                {
                    await ExecuteWithResult(sender, entry, message);
                    break;
                }
        }
    }

    private async Task LocalExecute(MicroEntryDelegate entry, MessageInvoke invokeMessage, MessageResult messageResult)
    {
        // execute every request in it's own service scope
        await using var scope = _scopeFactory.CreateAsyncScope();

        var callContextAccessor = scope.ServiceProvider.GetRequiredService<ICallContextAccessor>();

        // set context values
        callContextAccessor.Context = invokeMessage.Context;

        Task Handler() => ExecuteMethod(scope.ServiceProvider, entry, invokeMessage, messageResult);

        await scope.ServiceProvider
            .GetServices<IServiceMiddleware>()
            .Reverse()
            .Aggregate((ServiceHandlerDelegate)Handler, (next, pipeline) => () => pipeline.Execute(invokeMessage, next))();
    }

    private async Task ExecuteMethod(IServiceProvider serviceProvider, MicroEntryDelegate entry, MessageInvoke invokeMessage, MessageResult messageResult)
    {
        try
        {
            var data = await entry(serviceProvider, invokeMessage.Parameters).ConfigureAwait(false);

            if (data is not Task task)
            {
                messageResult.Content = data;
            }
            else
            {
                await task.ConfigureAwait(false);
                var taskType = task.GetType().GetTypeInfo();
                if (taskType.IsGenericType)
                {
                    var prop = taskType.GetProperty("Result");
                    if (prop != null)
                        messageResult.Content = prop.GetValue(task);
                }
            }
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Server error occured");

            messageResult.Code = 500;
            messageResult.Content = _exceptionTransformer.Wrap(ex);
        }
    }

    private async Task SendResult(IMessageSender sender, string messageId, Message result)
    {
        try
        {
            result.Id = messageId;
            await sender.Send(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send response message");
        }
    }

    private async Task ExecuteWithResult(IMessageSender sender, MicroEntryDelegate entry, MessageInvoke message)
    {
        var result = new MessageResult();
        await LocalExecute(entry, message, result);
        await SendResult(sender, message.Id, result);
    }

    private async Task ExecuteOneWay(IMessageSender sender, MicroEntryDelegate entry, MessageInvoke message)
    {
        var result = new MessageResult();

        await SendResult(sender, message.Id, result);

        await Task.Factory.StartNew(() =>
        {
            return LocalExecute(entry, message, result);
        }, TaskCreationOptions.LongRunning);
    }
}
