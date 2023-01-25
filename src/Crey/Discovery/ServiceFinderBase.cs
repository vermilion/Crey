using System.Collections.Concurrent;
using Crey.Extensions;

namespace Crey.Discovery;

internal abstract class ServiceFinderBase : IServiceFinder
{
    private ConcurrentDictionary<string, List<ServiceAddress>> _services = new();

    public async Task MonitorAliveServices(CancellationToken cancellationToken = default)
    {
        var services = await QueryAllAliveServices(cancellationToken);

        _services = new(services);
    }

    public Task<List<ServiceAddress>> QueryService(Type serviceType)
    {
        var serviceName = serviceType.Assembly.ServiceName();

        if (_services.TryGetValue(serviceName, out var addresses))
            return Task.FromResult(addresses);

        return Task.FromResult(new List<ServiceAddress>());
    }

    /// <summary>
    /// Allows to query all alive services from provider
    /// </summary>
    /// <param name="cancellationToken">Token <see cref="CancellationToken"/></param>
    /// <returns><see cref="Task{TResult}"/></returns>
    protected abstract Task<Dictionary<string, List<ServiceAddress>>> QueryAllAliveServices(CancellationToken cancellationToken = default);
}
