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

    public async Task Execute(IMessageSender sender, MessageInvoke invokeMessage)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(JsonHelper.ToJson(invokeMessage));

        var entry = _entryFactory.Find(invokeMessage.ServiceId);
        if (entry is null)
            throw new FaultException(ExceptionConstants.ServiceNotFound);

        var messageResult = new MessageResult();

        // execute every request in it's own service scope
        var scope = _scopeFactory.CreateScope();

        var callContextAccessor = scope.ServiceProvider.GetRequiredService<ICallContextAccessor>();

        // set context values
        callContextAccessor.Context = invokeMessage.Context;

        try
        {
            // start execution. filters -> middlewares -> actual method
            await ExecuteWithFilters(scope, entry, invokeMessage, messageResult);
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Server error occured");

            messageResult.IsSuccess = false;
            messageResult.Content = _exceptionTransformer.Wrap(ex);
        }
        finally
        {
            await SendResult(sender, invokeMessage.Id, messageResult);
        }
    }

    private async Task ExecuteWithFilters(IServiceScope scope, ServiceEntryInfo entry, MessageInvoke invokeMessage, MessageResult messageResult)
    {
        Task Handler() => ExecuteWithMiddlewares(scope, entry.Delegate, invokeMessage, messageResult);

        var filters = entry.MethodFilters.Select(x => (IServiceMethodFilter)scope.ServiceProvider.GetService(x));

        await filters
            .Reverse()
            .Aggregate((NextServiceDelegate)Handler, (next, pipeline) => () => pipeline.Execute(invokeMessage, next))();
    }

    private async Task ExecuteWithMiddlewares(IServiceScope scope, MicroEntryDelegate entry, MessageInvoke invokeMessage, MessageResult messageResult)
    {
        try
        {
            Task Handler() => ExecuteMethod(scope.ServiceProvider, entry, invokeMessage, messageResult);

            await scope.ServiceProvider
                .GetServices<IServiceMiddleware>()
                .Reverse()
                .Aggregate((NextServiceDelegate)Handler, (next, pipeline) => () => pipeline.Execute(invokeMessage, next))();
        }
        finally
        {
            scope?.Dispose();
        }
    }

    private async Task ExecuteMethod(IServiceProvider serviceProvider, MicroEntryDelegate entry, MessageInvoke invokeMessage, MessageResult messageResult)
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
                if (prop is not null)
                    messageResult.Content = prop.GetValue(task);
            }
        }
    }

    private async Task SendResult(IMessageSender sender, string messageId, Message result)
    {
        try
        {
            result.Id = messageId;
            await sender.Send(result).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send response message");
        }
    }
}