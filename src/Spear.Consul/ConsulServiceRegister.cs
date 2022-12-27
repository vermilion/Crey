using System.Net;
using System.Reflection;
using Consul;
using Microsoft.Extensions.Logging;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.ServiceDiscovery;
using Spear.Core.ServiceDiscovery.Constants;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Discovery.Consul;

public class ConsulServiceRegister : ServiceRegister
{
    private readonly string _consulServer;
    private readonly string _consulToken;
    private readonly List<string> _services;
    private readonly ILogger<ConsulServiceRegister> _logger;

    public ConsulServiceRegister(
        ILogger<ConsulServiceRegister> logger, 
        string server, string token)
    {
        _consulServer = server;
        _consulToken = token;
        _logger = logger;
        _services = new List<string>();
    }

    private IConsulClient CreateClient()
    {
        return new ConsulClient(cfg =>
        {
            cfg.Address = new Uri(_consulServer);
            if (!string.IsNullOrWhiteSpace(_consulToken))
                cfg.Token = _consulToken;
        });
    }

    public override async Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress)
    {
        using var client = CreateClient();

        foreach (var ass in assemblyList)
        {
            var serviceName = ass.ServiceName();
            var id = $"{serviceName}_{serverAddress}".Md5();

            await client.Agent.ServiceDeregister(id);

            var service = new AgentServiceRegistration
            {
                ID = id,
                Name = serviceName,
                Tags = new[] { ass.GetName().Version.ToString() },
                EnableTagOverride = true,
                Address = serverAddress.Host,
                Port = serverAddress.Port,
                Check = new AgentServiceCheck
                {
                    TCP = serverAddress.ToString(),
                    DeregisterCriticalServiceAfter = TimeSpan.FromDays(1),
                    Timeout = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10)
                },
                Meta = new Dictionary<string, string>
                {
                    { ServiceRouteConstants.KeyService, JsonHelper.ToJson(serverAddress) },
                    { ServiceRouteConstants.KeyVersion, ass.GetName().Version.ToString() }
                }
            };

            _services.Add(service.ID);

            var result = await client.Agent.ServiceRegister(service);
            if (result.StatusCode != HttpStatusCode.OK)
                _logger.LogWarning($"Service registration failed [{serviceName}@{serverAddress}]:{result.StatusCode}, {result.RequestTime}");
            else
                _logger.LogInformation($"Service registered [{serviceName}@{serverAddress}]");
        }
    }

    public override async Task Deregister()
    {
        using var client = CreateClient();

        foreach (var service in _services)
        {
            await client.Agent.ServiceDeregister(service);
            _logger.LogInformation($"Service deregistered [{service}]");
        }
    }
}
