using Microsoft.Extensions.Logging;
using Crey.ServiceDiscovery.Abstractions;
using Crey.ServiceDiscovery.Models;

namespace Crey.ServiceDiscovery;

public abstract class ServiceFinder : IServiceFinder
{
    protected readonly ILogger Logger;

    protected ServiceFinder(ILogger logger)
    {
        Logger = logger;
    }

    public abstract Task<List<ServiceAddress>> QueryService(Type serviceType);
}
