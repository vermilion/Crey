using System.Reflection;
using Crey.Helper;
using Crey.Message;
using Crey.Session;
using Microsoft.Extensions.Logging;

namespace Crey.Micro;

public class MicroExecutor : IMicroExecutor
{
    private readonly ILogger<MicroExecutor> _logger;
    private readonly IServiceProvider _provider;
    private readonly IMicroEntryFactory _entryFactory;
    private readonly ISessionValuesAccessor _sessionValuesAccessor;

    public MicroExecutor(
        ILogger<MicroExecutor> logger,
        IServiceProvider provider,
        ISessionValuesAccessor sessionValuesAccessor,
        IMicroEntryFactory entryFactory)
    {
        _provider = provider;
        _entryFactory = entryFactory;
        _sessionValuesAccessor = sessionValuesAccessor;
        _logger = logger;
    }

    private async Task LocalExecute(MicroEntryDelegate entry, InvokeMessage invokeMessage, MessageResult messageResult)
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

    private async Task SendResult(IMessageSender sender, string messageId, DMessage result)
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

    public async Task Execute(IMessageSender sender, InvokeMessage message)
    {
        if (message.Headers?.Any() == true)
        {
            foreach (var item in message.Headers)
            {
                _sessionValuesAccessor.Values.Add(item.Key, item.Value);
            }
        }

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(JsonHelper.ToJson(message));

        var entry = _entryFactory.Find(message.ServiceId);
        if (entry == null)
        {
            await SendResult(sender, message.Id, new MessageResult("Service not found"));
            return;
        }

        var isLongRunning = _sessionValuesAccessor.GetValue(MicroConstants.LongRunning, false);

        _logger.LogInformation($"Execute, LongRunning: {isLongRunning}");

        if (isLongRunning)
        {
            await ExecuteLongRunning(sender, entry, message);
            return;
        }

        await ExecuteWithResult(sender, entry, message);
    }

    private async Task ExecuteWithResult(IMessageSender sender, MicroEntryDelegate entry, InvokeMessage message)
    {
        var result = new MessageResult();
        await LocalExecute(entry, message, result);
        await SendResult(sender, message.Id, result);
    }

    private async Task ExecuteLongRunning(IMessageSender sender, MicroEntryDelegate entry, InvokeMessage message)
    {
        var result = new MessageResult();

        await SendResult(sender, message.Id, result);

        await Task.Factory.StartNew(async () =>
        {
            await LocalExecute(entry, message, result);
        }, TaskCreationOptions.LongRunning);
    }
}
