using Consul;
using Microsoft.Extensions.Logging;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.ServiceDiscovery;
using Spear.Core.ServiceDiscovery.Constants;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Discovery.Consul;

public class ConsulServiceFinder : ServiceFinder
{
    private readonly string _consulServer;
    private readonly string _consulToken;

    public ConsulServiceFinder(ILogger logger, string server, string token = null)
        : base(logger)
    {
        _consulServer = server;
        _consulToken = token;
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

    public override async Task<List<ServiceAddress>> QueryService(Type serviceType)
    {
        var services = new List<ServiceAddress>();

        using var client = CreateClient();
        var list = await client.Health.Service(serviceType.Assembly.ServiceName(), null, true);

        foreach (var entry in list.Response)
        {
            var service = entry.Service;

            if (service.Meta.TryGetValue(ServiceRouteConstants.KeyService, out var json))
            {
                var address = JsonHelper.FromJson<ServiceAddress>(json);
                if (address is not null)
                    services.Add(address);
            }
        }

        return services;
    }
}
