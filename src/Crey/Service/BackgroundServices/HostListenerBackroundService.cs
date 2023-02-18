using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Crey.Service;

internal class HostListenerBackroundService : BackgroundService
{
    private readonly IServiceHost _host;
    private readonly ServiceAddress _serviceAddress;
    private readonly ILogger<HostListenerBackroundService> _logger;

    public HostListenerBackroundService(
        IServiceHost host,
        IOptions<ServiceAddress> serviceAddressOptions,
        ILogger<HostListenerBackroundService> logger)
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
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Stopping listener service..");

        await base.StopAsync(cancellationToken);
        await _host.Stop();
    }
}
