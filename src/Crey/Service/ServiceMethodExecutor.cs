using System.Reflection;
using Crey.Helper;
using Microsoft.Extensions.Logging;

namespace Crey.Service;

public class ServiceMethodExecutor : IServiceMethodExecutor
{
    private readonly ILogger<ServiceMethodExecutor> _logger;
    private readonly IServiceProvider _provider;
    private readonly IServiceEntryFactory _entryFactory;
    private readonly ICallContextAccessor _sessionValuesAccessor;

    public ServiceMethodExecutor(
        ILogger<ServiceMethodExecutor> logger,
        IServiceProvider provider,
        ICallContextAccessor sessionValuesAccessor,
        IServiceEntryFactory entryFactory)
    {
        _provider = provider;
        _entryFactory = entryFactory;
        _sessionValuesAccessor = sessionValuesAccessor;
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

        _sessionValuesAccessor.Context = message.Context;

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
            var data = await entry(_provider, invokeMessage.Parameters);

            if (data is not Task task)
            {
                messageResult.Content = data;
            }
            else
            {
                await task;
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
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogError(ex, "Server error occured");

            messageResult.Message = ex.Message;
            messageResult.Code = 500;
        }
    }

    private async Task SendResult(IMessageSender sender, string messageId, Messages.Message result)
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
