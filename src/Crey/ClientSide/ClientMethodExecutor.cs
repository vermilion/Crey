using System.Net.Sockets;
using System.Reflection;
using Crey.Extensions;
using Crey.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Crey.ClientSide;

public class ClientMethodExecutor : IClientMethodExecutor
{
    private readonly ILogger<ClientMethodExecutor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IClientFactory _clientFactory;
    private readonly IServiceFinder _serviceFinder;

    public ClientMethodExecutor(
        ILogger<ClientMethodExecutor> logger,
        IServiceProvider serviceProvider,
        IClientFactory clientFactory,
        IServiceFinder serviceFinder)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _clientFactory = clientFactory;
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
        var services = await _serviceFinder.QueryServices(targetMethod.DeclaringType);

        ServiceAddress? service = null;

        var builder = Policy
            .Handle<Exception>(ex => ex.GetBaseException() is SocketException);

        // retry 3 times
        var policy = builder.RetryAsync(3, (ex, count) =>
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning($"{service}, {targetMethod.ServiceKey()}: retry #{count}, {ex.Format()}");

            services.Remove(service);
        });

        return await policy.ExecuteAsync(async () =>
        {
            if (!services.Any())
                throw new FaultException(ExceptionConstants.NoServicesFound);

            service = services.Random();

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