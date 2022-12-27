using System.Reflection;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Core.Helper;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;
using Spear.Core.Micro.Abstractions;
using Spear.Core.Micro.Models;
using Spear.Core.Session.Abstractions;
using Spear.Core.Session.Extensions;
using Spear.Core.Session.Models;

namespace Spear.Core.Micro;

public class MicroExecutor : IMicroExecutor
{
    private readonly ILogger<MicroExecutor> _logger;
    private readonly IMicroEntryFactory _entryFactory;
    private readonly IServiceProvider _provider;

    public MicroExecutor(ILogger<MicroExecutor> logger, IServiceProvider provider, IMicroEntryFactory entryFactory)
    {
        _entryFactory = entryFactory;
        _logger = logger;
        _provider = provider;
    }

    private async Task LocalExecute(MicroEntry entry, InvokeMessage invokeMessage, MessageResult messageResult)
    {
        try
        {
            if (entry.IsLongRunning)
            {
                await entry.Invoke(invokeMessage.Parameters);
            }
            else
            {
                var data = await entry.Invoke(invokeMessage.Parameters);
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
        var accessor = _provider.GetService<IPrincipalAccessor>();
        if (accessor != null && message.Headers != null && message.Headers.Any())
        {
            var session = new SessionDto();
            // Claims
            if (message.Headers.TryGetValue(SpearClaimTypes.HeaderUserId, out var userId))
                session.UserId = userId;
            // username
            if (message.Headers.TryGetValue(SpearClaimTypes.HeaderUserName, out var userName))
                session.UserName = HttpUtility.UrlDecode(userName);
            // role
            if (message.Headers.TryGetValue(SpearClaimTypes.HeaderRole, out var role))
                session.Role = HttpUtility.UrlDecode(role);
            accessor.SetSession(session);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(JsonHelper.ToJson(message));

        var entry = _entryFactory.Find(message.ServiceId);
        if (entry == null)
        {
            await SendResult(sender, message.Id, new MessageResult("Service not found"));
            return;
        }

        var result = new MessageResult();
        if (entry.IsLongRunning)
        {
            await SendResult(sender, message.Id, result);

            await Task.Factory.StartNew(async () =>
            {
                await LocalExecute(entry, message, result);
            }, TaskCreationOptions.LongRunning);
            return;
        }

        await LocalExecute(entry, message, result);
        await SendResult(sender, message.Id, result);
    }
}
