using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Polly;
using Crey.Exceptions;
using Crey.Extensions;
using Crey.Helper;
using Crey.Message.Models;
using Crey.Micro.Abstractions;
using Crey.Micro.Constants;
using Crey.Proxy.Abstractions;
using Crey.ServiceDiscovery.Abstractions;
using Crey.ServiceDiscovery.Extensions;
using Crey.ServiceDiscovery.Models;

namespace Crey.Proxy;

public class ClientProxy : IProxyProvider
{
    private readonly ILogger<ClientProxy> _logger;
    private readonly IMicroSession _session;
    private readonly IMicroClientFactory _clientFactory;
    private readonly IServiceFinder _serviceFinder;

    public ClientProxy(
        ILogger<ClientProxy> logger,
        IMicroSession session,
        IMicroClientFactory clientFactory,
        IServiceFinder finder)
    {
        _logger = logger;
        _session = session;
        _clientFactory = clientFactory;
        _serviceFinder = finder;
    }

    private async Task<MessageResult> ClientInvokeAsync(ServiceAddress serviceAddress, InvokeMessage message)
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

    private async Task<MessageResult> InternalInvoke(MethodInfo targetMethod, IDictionary<string, object> args)
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

    private InvokeMessage CreateMessage(MethodInfo targetMethod, IDictionary<string, object> args)
    {
        var headers = new Dictionary<string, string>
        {
            { MicroConstants.UserIp, IpAddressHelper.LocalIp() }
        };

        foreach (var item in _session.Values)
        {
            headers.Add(item.Key, item.Value);
        }

        return new InvokeMessage
        {
            ServiceId = targetMethod.ServiceKey(),
            Headers = headers,
            Parameters = args
        };
    }

    public object Invoke(MethodInfo method, IDictionary<string, object> parameters)
    {
        var result = InternalInvoke(method, parameters).ConfigureAwait(false).GetAwaiter().GetResult();
        return result.Content;
    }

    public Task InvokeAsync(MethodInfo method, IDictionary<string, object> parameters)
    {
        return InternalInvoke(method, parameters);
    }

    public async Task<T> InvokeAsync<T>(MethodInfo method, IDictionary<string, object> parameters)
    {
        var result = await InternalInvoke(method, parameters);
        return (T)result.Content;
    }
}
