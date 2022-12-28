using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.BackgroundServices;

public class HostListenerBackroungService : BackgroundService
{
    private readonly IMicroHost _host;
    private readonly ServiceAddress _serviceAddress;
    private readonly ILogger<HostListenerBackroungService> _logger;

    public HostListenerBackroungService(
        IMicroHost host,
        IOptions<ServiceAddress> serviceAddressOptions,
        ILogger<HostListenerBackroungService> logger)
    {
        _host = host;
        _serviceAddress = serviceAddressOptions.Value;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _host.Start(_serviceAddress);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping listener service..");
        await base.StopAsync(cancellationToken);
        await _host.Stop();
    }
}
