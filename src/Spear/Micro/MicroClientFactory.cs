using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.Micro;

public abstract class MicroClientFactory : IMicroClientFactory, IDisposable
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly ILogger Logger;
    protected readonly IServiceProvider Provider;
    protected readonly IMicroExecutor MicroExecutor;

    private readonly ConcurrentDictionary<ServiceAddress, Lazy<Task<IMicroClient>>> _clients = new ();

    protected MicroClientFactory(ILoggerFactory loggerFactory, IServiceProvider provider, IMicroExecutor microExecutor)
    {
        Provider = provider;
        MicroExecutor = microExecutor;
        LoggerFactory = loggerFactory;
        Logger = loggerFactory.CreateLogger(GetType());
    }

    protected abstract Task<IMicroClient> Create(ServiceAddress address);

    protected void Remove(ServiceAddress address)
    {
        Logger.LogInformation($"Client removed: {address}");
        _clients.TryRemove(address, out _);
    }

    public async Task<IMicroClient> CreateClient(ServiceAddress serviceAddress)
    {
        try
        {
            var lazyClient = _clients.GetOrAdd(serviceAddress,
                key => new Lazy<Task<IMicroClient>>(async () =>
                {
                    Logger.LogInformation($"Client created：{key}");
                    return await Create(key);
                }));

            return await lazyClient.Value;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to create client: {serviceAddress}");
            Remove(serviceAddress);
            throw;
        }
    }

    public void Dispose()
    {
        foreach (var client in _clients.Values.Where(i => i.IsValueCreated))
        {
            (client.Value as IDisposable)?.Dispose();
        }
    }
}
