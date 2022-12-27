using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Spear.Core.Exceptions;
using Spear.Core.Helper;
using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;
using Spear.Core.Micro.Abstractions;

namespace Spear.Core.Micro
{
    /// <summary> 默认服务客户端 </summary>
    public class MicroClient : IMicroClient, IDisposable
    {
        private readonly IMessageSender _sender;
        private readonly IMessageListener _listener;
        private readonly IMicroExecutor _executor;
        private readonly ILogger<MicroClient> _logger;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<MessageResult>> _resultDictionary;

        public MicroClient(IMessageSender sender, IMessageListener listener, IMicroExecutor executor, ILoggerFactory loggerFactory)
        {
            _sender = sender;
            _listener = listener;
            _executor = executor;
            _logger = loggerFactory.CreateLogger<MicroClient>();
            _resultDictionary = new ConcurrentDictionary<string, TaskCompletionSource<MessageResult>>();
            listener.Received += ListenerOnReceived;
        }

        private async Task ListenerOnReceived(IMessageSender sender, DMessage message)
        {
            if (!_resultDictionary.TryGetValue(message.Id, out var task))
                return;

            if (message is MessageResult result)
            {
                if (result.Code != 200)
                {
                    task.TrySetException(new SpearException(result.Message, result.Code));
                }
                else
                {
                    task.SetResult(result);
                }
            }

            if (_executor != null && message is InvokeMessage invokeMessage)
                await _executor.Execute(sender, invokeMessage);
        }

        private async Task<MessageResult> RegisterCallbackAsync(string messageId)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"准备获取Id为：{messageId}的响应内容。");

            var task = new TaskCompletionSource<MessageResult>();
            _resultDictionary.TryAdd(messageId, task);

            try
            {
                var result = await task.Task;
                return result;
            }
            finally
            {
                //删除回调任务
                _resultDictionary.TryRemove(messageId, out _);
            }
        }

        public async Task<MessageResult> Send(InvokeMessage message)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Prepare to send request");

                var callback = RegisterCallbackAsync(message.Id);

                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug($"{_sender.GetType()}:send :{JsonHelper.ToJson(message)}");

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
}
