using System.Collections.Concurrent;
using System.Diagnostics;
using Crey.Helpers;
using Microsoft.Extensions.Logging;

namespace Crey.ClientSide;

public class Client : IClient, IDisposable
{
    private readonly IMessageSender _sender;
    private readonly IMessageListener _listener;
    private readonly IExceptionConverter _exceptionConverter;
    private readonly ILogger<Client> _logger;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<MessageResult>> _resultDictionary = new();

    public Client(
        IMessageSender sender, 
        IMessageListener listener,
        IExceptionConverter exceptionConverter,
        ILoggerFactory loggerFactory)
    {
        _sender = sender;
        _listener = listener;
        _exceptionConverter = exceptionConverter;
        _logger = loggerFactory.CreateLogger<Client>();
        listener.Received += ListenerOnReceived;
    }

    private Task ListenerOnReceived(IMessageSender sender, Message message)
    {
        if (!_resultDictionary.TryGetValue(message.Id, out var task))
            return Task.CompletedTask;

        if (message is MessageResult result)
        {
            if (result.IsSuccess is false)
            {
                var ex = _exceptionConverter.Unwrap(result.Content as MessageException);
                task.TrySetException(ex);
            }
            else
            {
                task.SetResult(result);
            }
        }

        return Task.CompletedTask;
    }

    private async Task<MessageResult> RegisterCallbackAsync(string messageId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug($"Callback added for message id：{messageId}");

        var task = new TaskCompletionSource<MessageResult>();
        _resultDictionary.TryAdd(messageId, task);

        try
        {
            var result = await task.Task;
            return result;
        }
        finally
        {
            _resultDictionary.TryRemove(messageId, out _);
        }
    }

    public async Task<MessageResult> Send(MessageInvoke message)
    {
        var watch = Stopwatch.StartNew();

        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Prepare to send request");

            var callback = RegisterCallbackAsync(message.Id);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"{_sender.GetType()}:send {JsonHelper.ToJson(message)}");

            // sending
            await _sender.Send(message);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Request sent");

            return await callback;
        }
        finally
        {
            watch.Stop();

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"send message {watch.ElapsedMilliseconds} ms");
        }
    }

    public void Dispose()
    {
        (_sender as IDisposable)?.Dispose();
        (_listener as IDisposable)?.Dispose();

        foreach (var taskCompletionSource in _resultDictionary.Values)
        {
            taskCompletionSource.TrySetCanceled();
        }
    }
}
