using Microsoft.Extensions.Logging;

namespace Crey.Service;

public class ServiceHost : IServiceHost, IDisposable
{
    private readonly ILogger<ServiceHost> _logger;
    private readonly ITransportListener _transportListener;

    public ServiceHost(
        ILogger<ServiceHost> logger,
        ITransportListener transportListener)
    {
        _logger = logger;
        _transportListener = transportListener;
    }

    public async Task Start(ServiceAddress serviceAddress)
    {
        try
        {
            await _transportListener.Start(serviceAddress);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Service started at： {serviceAddress}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task Stop()
    {
        await _transportListener.Stop();
        Console.WriteLine("Service Stopped");
    }

    public void Dispose()
    {
        (_transportListener as IDisposable)?.Dispose();
        Task.Run(Stop).Wait();
    }
}
