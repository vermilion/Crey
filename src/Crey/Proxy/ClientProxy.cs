﻿using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using Crey.Discovery;
using Crey.Exceptions;
using Crey.Extensions;
using Crey.Helper;
using Crey.Message;
using Crey.Micro;
using Crey.Session;
using Microsoft.Extensions.Logging;
using Polly;

namespace Crey.Proxy;

public class ClientProxy : IProxyProvider
{
    private readonly ILogger<ClientProxy> _logger;
    private readonly IMicroClientFactory _clientFactory;
    private readonly IServiceFinder _serviceFinder;

    public ClientProxy(
        ILogger<ClientProxy> logger,
        IMicroClientFactory clientFactory,
        IServiceFinder serviceFinder)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _serviceFinder = serviceFinder;
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
        var context = new InvokeMethodContext(InvokeMethodContextProvider.Current);
        context.Headers.Add(MicroConstants.UserIp, IpAddressHelper.LocalIp()?.ToString());

        return new InvokeMessage
        {
            ServiceId = targetMethod.ServiceKey(),
            Parameters = args,
            Context = context
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
