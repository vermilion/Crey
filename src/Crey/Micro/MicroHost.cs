using Microsoft.Extensions.Logging;
using Psi.Micro.Abstractions;
using Psi.ServiceDiscovery.Abstractions;
using Psi.ServiceDiscovery.Models;

namespace Psi.Micro;

public class MicroHost : MicroHostBase
{
    private readonly IServiceRegister _serviceRegister;
    private readonly IMicroEntryFactory _entryFactory;
    private readonly ILogger<MicroHost> _logger;

    public MicroHost(
        IServiceProvider serviceProvider,
        IMicroListener microListener,
        IServiceRegister serviceRegister,
        IMicroEntryFactory entryFactory,
        ILoggerFactory loggerFactory)
        : base(serviceProvider, microListener, loggerFactory)
    {
        _serviceRegister = serviceRegister;
        _entryFactory = entryFactory;
        _logger = loggerFactory.CreateLogger<MicroHost>();
    }

    public override void Dispose()
    {
        (MicroListener as IDisposable)?.Dispose();
    }

    public override async Task Start(ServiceAddress serviceAddress)
    {
        try
        {
            await MicroListener.Start(serviceAddress);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Service started at：{serviceAddress}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        var assemblies = _entryFactory.GetContracts();
        await _serviceRegister.Register(assemblies, serviceAddress);
    }

    public override async Task Stop()
    {
        await _serviceRegister.Deregister();
        await MicroListener.Stop();
        Console.WriteLine("Service Stopped");
    }
}
