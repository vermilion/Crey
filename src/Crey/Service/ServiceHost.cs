using Microsoft.Extensions.Logging;

namespace Crey.Service;

public class ServiceHost : IServiceHost, IDisposable
{
    private readonly ILogger<ServiceHost> _logger;
    private readonly IServiceRegister _serviceRegister;
    private readonly IServiceEntryFactory _entryFactory;
    private readonly ITransportListener _transportListener;

    public ServiceHost(
        ILogger<ServiceHost> logger,
        ITransportListener transportListener,
        IServiceRegister serviceRegister,
        IServiceEntryFactory entryFactory)
    {
        _logger = logger;
        _serviceRegister = serviceRegister;
        _entryFactory = entryFactory;
        _transportListener = transportListener;
    }

    public async Task Start(ServiceAddress serviceAddress)
    {
        try
        {
            await _transportListener.Start(serviceAddress);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Service started at： {serviceAddress}");

            var assemblies = _entryFactory.GetContracts();
            await _serviceRegister.Register(assemblies, serviceAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task Stop()
    {
        await _serviceRegister.Deregister();
        await _transportListener.Stop();
        Console.WriteLine("Service Stopped");
    }

    public void Dispose()
    {
        (_transportListener as IDisposable)?.Dispose();
        Task.Run(Stop).Wait();
    }
}
