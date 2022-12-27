using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.Message.Models;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery.Abstractions;
using Spear.Core.ServiceDiscovery.Extensions;
using Spear.Core.ServiceDiscovery.Models;
using Spear.Core.Session.Abstractions;
using Spear.Core.Session.Extensions;
using Spear.Core.Session.Models;
using Spear.ProxyGenerator.Abstractions;

namespace Spear.Core.Proxy;

public class ClientProxy : IProxyProvider
{
    private readonly ILogger<ClientProxy> _logger;
    private readonly IMicroClientFactory _clientFactory;
    private readonly IServiceProvider _provider;
    private readonly IServiceFinder _serviceFinder;

    public ClientProxy(
        ILogger<ClientProxy> logger,
        IServiceProvider provider,
        IMicroClientFactory clientFactory,
        IServiceFinder finder)
    {
        _logger = logger;
        _provider = provider;
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

        var invokeMessage = Create(targetMethod, args);
        ServiceAddress service = null;

        var builder = Policy
            .Handle<Exception>(ex =>
                ex.GetBaseException() is SocketException ||
                ex.GetBaseException() is HttpRequestException ||
                (ex.GetBaseException() is SpearException spearEx && spearEx.Code == ErrorCodes.SystemError))
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

    private InvokeMessage Create(MethodInfo targetMethod, IDictionary<string, object> args)
    {
        var localIp = IpAddressHelper.LocalIp();
        var headers = new Dictionary<string, string>
        {
            { SpearClaimTypes.HeaderUserIp, localIp },
            { SpearClaimTypes.HeaderUserAgent, "spear-client" }
        };

        var session = _provider.GetService<IMicroSession>();
        if (session != null)
        {
            if (session.UserId != null)
            {
                headers.Add(SpearClaimTypes.HeaderUserId, session.GetUserId<string>());
                headers.Add(SpearClaimTypes.HeaderUserName, HttpUtility.UrlEncode(session.UserName ?? string.Empty));
                headers.Add(SpearClaimTypes.HeaderRole, HttpUtility.UrlEncode(session.Role ?? string.Empty));
            }
        }

        var serviceId = targetMethod.ServiceKey();
        var invokeMessage = new InvokeMessage
        {
            ServiceId = serviceId,
            Headers = headers,
            Parameters = args
        };
        return invokeMessage;
    }

    public object Invoke(MethodInfo method, IDictionary<string, object> parameters, object key = null)
    {
        var result = InternalInvoke(method, parameters).ConfigureAwait(false).GetAwaiter().GetResult();
        return result.Content;
    }

    public Task InvokeAsync(MethodInfo method, IDictionary<string, object> parameters, object key = null)
    {
        return InternalInvoke(method, parameters);
    }

    public async Task<T> InvokeAsync<T>(MethodInfo method, IDictionary<string, object> parameters, object key = null)
    {
        var result = await InternalInvoke(method, parameters);
        return (T)result.Content;
    }
}
