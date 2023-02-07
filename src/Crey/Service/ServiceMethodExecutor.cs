using System.Reflection;
using Crey.Exceptions;
using Crey.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crey.Service;

public class ServiceMethodExecutor : IServiceMethodExecutor
{
    private readonly ILogger<ServiceMethodExecutor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceEntryFactory _entryFactory;
    private readonly ICallContextAccessor _sessionValuesAccessor;
    private readonly IExceptionConverter _exceptionTransformer;

    public ServiceMethodExecutor(
        ILogger<ServiceMethodExecutor> logger,
        IServiceProvider serviceProvider,
        ICallContextAccessor sessionValuesAccessor,
        IExceptionConverter exceptionTransformer,
        IServiceEntryFactory entryFactory)
    {
        _serviceProvider = serviceProvider;
        _sessionValuesAccessor = sessionValuesAccessor;
        _exceptionTransformer = exceptionTransformer;
        _entryFactory = entryFactory;
        _logger = logger;
    }

    public async Task Execute(IMessageSender sender, MessageInvoke message)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(JsonHelper.ToJson(message));

        var entry = _entryFactory.Find(message.ServiceId);
        if (entry == null)
        {
            await SendResult(sender, message.Id, new MessageResult("Service not found"));
            return;
        }

        // set context values
        _sessionValuesAccessor.Context = message.Context;

        Task Handler() => ExecuteInternal(sender, message, entry);

        await _serviceProvider
            .GetServices<IServiceMiddleware>()
            .Reverse()
            .Aggregate((ServiceHandlerDelegate)Handler, (next, pipeline) => () => pipeline.Execute(message, next))();
    }

    private async Task ExecuteInternal(IMessageSender sender, MessageInvoke message, MicroEntryDelegate entry)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation($"Execute, InvokeType: {message.Context.Type}");

        if (message.Context.Type == MessageInvokeContextType.OneWay)
        {
            await ExecuteOneWay(sender, entry, message);
            return;
        }

        await ExecuteWithResult(sender, entry, message);
    }

    private async Task LocalExecute(MicroEntryDelegate entry, MessageInvoke invokeMessage, MessageResult messageResult)
    {
        try
        {
            var data = await entry(_serviceProvider, invokeMessage.Parameters).ConfigureAwait(false);

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

        await Task.Factory.StartNew(async () =>
        {
            await LocalExecute(entry, message, result);
        }, TaskCreationOptions.LongRunning);
    }
}
