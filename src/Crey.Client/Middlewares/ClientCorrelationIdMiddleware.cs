using Crey.CallContext;
using Crey.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crey.ClientSide;

internal class ClientCorrelationIdMiddleware : IClientMiddleware
{
    private readonly ILogger<ClientCorrelationIdMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ClientCorrelationIdMiddleware(
        ILogger<ClientCorrelationIdMiddleware> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task<MessageResult> Execute(MessageInvoke message, ClientHandlerDelegate next)
    {
        // try get accessor if present
        var callContextAccessor = _serviceProvider.GetService<ICallContextAccessor>();

        if (callContextAccessor is not null)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Found correlationId: {callContextAccessor.Context.CorrelationId}");

            message.Context.CorrelationId = callContextAccessor.Context.CorrelationId;
        }

        return next();
    }
}
