﻿using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.ServiceDiscovery;
using Spear.Core.ServiceDiscovery.Constants;
using Spear.Core.ServiceDiscovery.Models;
using Spear.Discovery.Consul.Options;

namespace Spear.Discovery.Consul;

public class ConsulServiceFinder : ServiceFinder
{
    private readonly ConsulOptions _options;

    public ConsulServiceFinder(
        ILogger<ConsulServiceRegister> logger,
        IOptions<ConsulOptions> options)
        : base(logger)
    {
        _options = options.Value;
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
