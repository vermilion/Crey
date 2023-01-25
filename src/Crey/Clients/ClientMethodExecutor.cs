using System.Net.Sockets;
using System.Reflection;
using Crey.Exceptions;
using Crey.Extensions;
using Crey.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Crey.Clients;

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
        var services = await _serviceFinder.QueryService(targetMethod.DeclaringType);

        // fail fast if no services present
        if (!services.Any())
            throw new FaultException("No services found alive");

        var invokeMessage = CreateMessage(targetMethod, args);

        Task<MessageResult> Handler() => ExecuteInternal(services, targetMethod, invokeMessage);

        return await _serviceProvider
            .GetServices<IClientMiddleware>()
            .Reverse()
            .Aggregate((ClientHandlerDelegate)Handler, (next, pipeline) => () => pipeline.Execute(invokeMessage, next))();
    }

    private async Task<MessageResult> ExecuteInternal(List<ServiceAddress> services, MethodInfo targetMethod, MessageInvoke message)
    {
        ServiceAddress? service = null;

        var builder = Policy
            .Handle<Exception>(ex =>
                ex.GetBaseException() is SocketException ||
                ex.GetBaseException() is FaultException faultEx && faultEx.Code == ErrorCodes.SystemError)
            .OrResult<MessageResult>(r => r.Code != 200);

        // retry 3 times
        var policy = builder.RetryAsync(3, (result, count) =>
        {
            _logger.LogWarning(result.Exception != null
                ? $"{service},{targetMethod.ServiceKey()}:retry,{count},{result.Exception.Format()}"
                : $"{service},{targetMethod.ServiceKey()}:retry,{count},{result.Result.Code}");

            services.Remove(service);
        });

        return await policy.ExecuteAsync(async () =>
        {
            if (!services.Any())
                throw ErrorCodes.NoService.CodeException();

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