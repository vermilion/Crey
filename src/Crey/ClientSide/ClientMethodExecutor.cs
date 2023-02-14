using System.Net.Sockets;
using System.Reflection;
using Crey.Extensions;
using Crey.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Crey.ClientSide;

internal class ClientMethodExecutor : IClientMethodExecutor
{
    private readonly ILogger<ClientMethodExecutor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IClientFactory _clientFactory;
    private readonly IClientRetryPolicyProvider _retryPolicyProvider;
    private readonly IClientLoadBalancingStrategy _loadBalancingStrategy;
    private readonly IServiceFinder _serviceFinder;

    public ClientMethodExecutor(
        ILogger<ClientMethodExecutor> logger,
        IServiceProvider serviceProvider,
        IClientFactory clientFactory,
        IClientRetryPolicyProvider retryPolicyProvider,
        IClientLoadBalancingStrategy loadBalancingStrategy,
        IServiceFinder serviceFinder)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _clientFactory = clientFactory;
        _retryPolicyProvider = retryPolicyProvider;
        _loadBalancingStrategy = loadBalancingStrategy;
        _serviceFinder = serviceFinder;
    }

    public async Task<MessageResult> Execute(MethodInfo targetMethod, IDictionary<string, object> args)
    {
        var invokeMessage = CreateMessage(targetMethod, args);

        Task<MessageResult> Handler() => ExecuteInternal(targetMethod, invokeMessage);

        return await _serviceProvider
            .GetServices<IClientMiddleware>()
            .Reverse()
            .Aggregate((ClientHandlerDelegate)Handler, (next, pipeline) => () => pipeline.Execute(invokeMessage, next))();
    }

    private async Task<MessageResult> ExecuteInternal(MethodInfo targetMethod, MessageInvoke message)
    {
        var policy = _retryPolicyProvider.Provide((ex, timeSpan, count, context) =>
        {
            context.TryGetValue("service", out var service);

            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning($"{service}, {targetMethod.ServiceKey()}: retry #{count}, {ex.Format()}");
        });

        var builder = Policy
            .Handle<Exception>(ex => ex.GetBaseException() is SocketException);

        var services = await _serviceFinder.QueryServices(targetMethod.DeclaringType);

        return await policy.ExecuteAsync(async () =>
        {
            if (!services.Any())
                throw new FaultException(ExceptionConstants.NoServicesFound);

            var service = await _loadBalancingStrategy.GetNextService(services);

            if (service is null)
                throw new FaultException(ExceptionConstants.ServiceCallFailed);

            var client = await _clientFactory.CreateClient(service);
            return await client.Send(message);
        });
    }

    private MessageInvoke CreateMessage(MethodInfo targetMethod, IDictionary<string, object> args)
    {
        var context = new MessageInvokeContext(InvokeMethodContextProvider.Context);
        context.Headers.Add(CoreConstants.UserIp, IpAddressHelper.LocalIp()?.ToString());

        return new MessageInvoke
        {
            ServiceId = targetMethod.ServiceKey(),
            Parameters = args,
            Context = context
        };
    }
}