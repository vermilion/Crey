using System.Net;
using System.Reflection;
using Consul;
using Crey.Extensions;
using Crey.Extensions.StringExtension;
using Crey.Helpers;
using Crey.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Crey.Discovery.Consul;

internal class ConsulServiceRegister : IServiceRegister
{
    private readonly List<string> _services = new();
    private readonly ConsulOptions _options;
    private readonly ILogger<ConsulServiceRegister> _logger;

    public ConsulServiceRegister(
        ILogger<ConsulServiceRegister> logger,
        IOptions<ConsulOptions> options)
    {
        _options = options.Value;
        _logger = logger;
    }

    private IConsulClient CreateClient()
    {
        return new ConsulClient(cfg =>
        {
            cfg.Address = new Uri(_options.Server);
            if (!string.IsNullOrWhiteSpace(_options.Token))
                cfg.Token = _options.Token;
        });
    }

    public async Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress)
    {
        using var client = CreateClient();

        foreach (var assembly in assemblyList)
        {
            var serviceName = assembly.ServiceName();
            var id = $"{serviceName}_{serverAddress}".Md5();

            await client.Agent.ServiceDeregister(id);

            var service = new AgentServiceRegistration
            {
                ID = id,
                Name = serviceName,
                Tags = new[] { assembly.GetName().Version.ToString() },
                EnableTagOverride = true,
                Address = serverAddress.Host,
                Port = serverAddress.Port,
                Check = new AgentServiceCheck
                {
                    TCP = serverAddress.ToString(),
                    DeregisterCriticalServiceAfter = _options.Service.Check.DeregisterCriticalServiceAfter,
                    Timeout = TimeSpan.FromSeconds(_options.Service.Check.Timeout),
                    Interval = TimeSpan.FromSeconds(_options.Service.Check.Interval)
                },
                Meta = new Dictionary<string, string>
                {
                    [ConsulRouteConstants.KeyService] = JsonHelper.ToJson(serverAddress),
                    [ConsulRouteConstants.KeyVersion] = assembly.GetName().Version.ToString()
                }
            };

            // merge with additional tags
            if (_options.Service.Tags is not null)
            {
                service.Tags = service.Tags
                    .Union(_options.Service.Tags)
                    .ToArray();
            }

            // merge with additional metadata values
            if (_options.Service.Meta is not null)
            {
                service.Meta = service.Meta
                    .Union(_options.Service.Meta)
                    .ToDictionary(x => x.Key, y => y.Value);
            }

            _services.Add(service.ID);

            var result = await client.Agent.ServiceRegister(service);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning($"Service registration failed [{serviceName}@{serverAddress}]:{result.StatusCode}, {result.RequestTime}");
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation($"Service registered [{serviceName}@{serverAddress}]");
            }
        }
    }

    public async Task Deregister()
    {
        using var client = CreateClient();

        foreach (var service in _services)
        {
            await client.Agent.ServiceDeregister(service);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Service deregistered [{service}]");
        }
    }
}
