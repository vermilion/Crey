using Consul;
using Crey.Extensions;
using Crey.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Crey.Discovery.Consul;

internal class ConsulServiceFinder : IServiceFinder
{
    private readonly ILogger<ConsulServiceRegister> _logger;
    private readonly ConsulOptions _options;

    public ConsulServiceFinder(
        ILogger<ConsulServiceRegister> logger,
        IOptions<ConsulOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<List<ServiceAddress>> QueryServices(Type serviceType)
    {
        var services = new List<ServiceAddress>();
        using var client = CreateClient();
        var list = await client.Health.Service(serviceType.Assembly.ServiceName(), null, true);
        foreach (var entry in list.Response)
        {
            var service = entry.Service;
            if (service.Meta.TryGetValue(ConsulRouteConstants.KeyService, out var json))
            {
                var address = JsonHelper.FromJson<ServiceAddress>(json);
                if (address is not null)
                    services.Add(address);
            }
        }

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug($"Found {services.Count} alive services");

        return services;
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
}
