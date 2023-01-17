using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Crey.Service;

internal class ServiceLoggingMiddleware : IServiceMiddleware
{
    private readonly ILogger<ServiceLoggingMiddleware> _logger;

    public ServiceLoggingMiddleware(ILogger<ServiceLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Execute(MessageInvoke message, ServiceHandlerDelegate next)
    {
        var watch = Stopwatch.StartNew();

        try
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = message.Context.CorrelationId,
                ["OperationName"] = message.ServiceId
            });

            await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occured during method execution");
            throw;
        }
        finally
        {
            watch.Stop();
            _logger.LogInformation($"Method executed in {watch.ElapsedMilliseconds}ms");
        }
    }
}
