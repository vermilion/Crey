using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Crey.Discovery;

internal class ServiceRegisterBackroundService : BackgroundService
{
    private readonly IServiceRegister _serviceRegister;
    private readonly IServiceEntryFactory _entryFactory;
    private readonly ServiceAddress _serviceAddress;
    private readonly ILogger<ServiceRegisterBackroundService> _logger;

    public ServiceRegisterBackroundService(
        IServiceRegister serviceRegister,
        IServiceEntryFactory entryFactory,
        IOptions<ServiceAddress> serviceAddressOptions,
        ILogger<ServiceRegisterBackroundService> logger)
    {
        _serviceRegister = serviceRegister;
        _entryFactory = entryFactory;
        _serviceAddress = serviceAddressOptions.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var assemblies = _entryFactory.GetContracts();
            await _serviceRegister.Register(assemblies, _serviceAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping registration service..");
        await base.StopAsync(cancellationToken);
        await _serviceRegister.Deregister();
    }
}
