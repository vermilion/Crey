using Consul;
using Crey.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Crey.Discovery.Consul;

internal class ConsulServiceFinder : ServiceFinderBase
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

    private IConsulClient CreateClient()
    {
        return new ConsulClient(cfg =>
        {
            cfg.Address = new Uri(_options.Server);
            if (!string.IsNullOrWhiteSpace(_options.Token))
                cfg.Token = _options.Token;
        });
    }

    protected override async Task<Dictionary<string, List<ServiceAddress>>> QueryAllAliveServices(CancellationToken cancellationToken = default)
    {
        var services = new Dictionary<string, List<ServiceAddress>>();

        using var client = CreateClient();

        var list = await client.Catalog.Services(cancellationToken);

        foreach (var service in list.Response)
        {
            var healthCheckList = await client.Health.Service(service.Key, null, true, cancellationToken);

            // service exists - add key to dictionary
            services.Add(service.Key, new List<ServiceAddress>());

            foreach (var entry in healthCheckList.Response)
            {
                var serviceInfo = entry.Service;

                if (serviceInfo.Meta.TryGetValue(ConsulRouteConstants.KeyService, out var json))
                {
                    var address = JsonHelper.FromJson<ServiceAddress>(json);
                    if (address is not null)
                        services[service.Key].Add(address);
                }
            }
        }

        return services;
    }
}
