using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Crey.Clients;

public abstract class ClientFactory : IClientFactory, IDisposable
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly ILogger Logger;
    protected readonly IServiceProvider Provider;

    private readonly ConcurrentDictionary<string, Lazy<Task<IClient>>> _clients = new();

    protected ClientFactory(ILoggerFactory loggerFactory, IServiceProvider provider)
    {
        Provider = provider;
        LoggerFactory = loggerFactory;
        Logger = loggerFactory.CreateLogger(GetType());
    }

    protected abstract Task<IClient> Create(ServiceAddress address);

    protected void Remove(ServiceAddress address)
    {
        Logger.LogInformation($"Client removed: {address}");
        _clients.TryRemove(address.ToString(), out _);
    }

    public async Task<IClient> CreateClient(ServiceAddress serviceAddress)
    {
        try
        {
            var lazyClient = _clients.GetOrAdd(serviceAddress.ToString(),
                key => new Lazy<Task<IClient>>(async () =>
                {
                    Logger.LogInformation($"Client created：{key}");
                    return await Create(serviceAddress);
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
