using System.Diagnostics;
using Crey.Messages;
using Microsoft.Extensions.Logging;

namespace Crey.ClientSide;

public class ClientLoggingMiddleware : IClientMiddleware
{
    private readonly ILogger<ClientLoggingMiddleware> _logger;

    public ClientLoggingMiddleware(ILogger<ClientLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task<MessageResult> Execute(MessageInvoke message, ClientHandlerDelegate next)
    {
        var watch = Stopwatch.StartNew();

        try
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = message.Context.CorrelationId,
                ["OperationName"] = message.ServiceId
            });

            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occured during method execution");
            throw;
        }
        finally
        {
            watch.Stop();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Method executed in {watch.ElapsedMilliseconds}ms");
        }
    }
}
