using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using Crey.Clients;
using Crey.Exceptions;
using Crey.Extensions;
using Crey.Helper;
using Microsoft.Extensions.Logging;
using Polly;

namespace Crey.Clients;

public class ClientMethodExecutor : IClientMethodExecutor
{
    private readonly ILogger<ClientMethodExecutor> _logger;
    private readonly IClientFactory _clientFactory;
    private readonly IServiceFinder _serviceFinder;

    public ClientMethodExecutor(
        ILogger<ClientMethodExecutor> logger,
        IClientFactory clientFactory,
        IServiceFinder serviceFinder)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _serviceFinder = serviceFinder;
    }

    public async Task<MessageResult> Execute(MethodInfo targetMethod, IDictionary<string, object> args)
    {
        var serviceType = targetMethod.DeclaringType;

        var services = await _serviceFinder.QueryService(serviceType);

        if (!services.Any())
            throw ErrorCodes.NoService.CodeException();

        var invokeMessage = CreateMessage(targetMethod, args);
        ServiceAddress? service = null;

        var builder = Policy
            .Handle<Exception>(ex =>
                ex.GetBaseException() is SocketException ||
                ex.GetBaseException() is HttpRequestException ||
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

            return await ClientInvokeAsync(service, invokeMessage);
        });
    }

    private async Task<MessageResult> ClientInvokeAsync(ServiceAddress serviceAddress, MessageInvoke message)
    {
        var watch = Stopwatch.StartNew();

        try
        {
            var client = await _clientFactory.CreateClient(serviceAddress);
            return await client.Send(message);
        }
        finally
        {
            watch.Stop();
            _logger.LogInformation($"ClientInvokeAsync {watch.ElapsedMilliseconds} ms");
        }
    }

    private MessageInvoke CreateMessage(MethodInfo targetMethod, IDictionary<string, object> args)
    {
        var context = new MessageInvokeContext(InvokeMethodContextProvider.Context);
        context.Headers.Add(MicroConstants.UserIp, IpAddressHelper.LocalIp()?.ToString());

        return new MessageInvoke
        {
            ServiceId = targetMethod.ServiceKey(),
            Parameters = args,
            Context = context
        };
    }
}