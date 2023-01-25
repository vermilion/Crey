using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Crey.Discovery;

internal class ServiceMonitorBackroundService : BackgroundService
{
    private readonly IServiceFinder _serviceFinder;
    private readonly DiscoveryOptions _options;
    private readonly ILogger<ServiceRegisterBackroundService> _logger;

    public ServiceMonitorBackroundService(
        IServiceFinder serviceFinder,
        IOptions<DiscoveryOptions> options,
        ILogger<ServiceRegisterBackroundService> logger)
    {
        _serviceFinder = serviceFinder;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await PeriodicAsync(async () =>
        {
            await DoWork(stoppingToken);
        }, TimeSpan.FromSeconds(_options.FetchInterval), stoppingToken);
    }

    private async Task DoWork(CancellationToken cancellationToken = default)
    {
        await _serviceFinder.MonitorAliveServices(cancellationToken);
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("ServiceMonitor is stopping.");

        return Task.CompletedTask;
    }

    private static async Task PeriodicAsync(Func<Task> action, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await action();

            if (cancellationToken.IsCancellationRequested)
                break;

            await Task.Delay(interval, cancellationToken);
        }
    }
}