using Microsoft.Extensions.Logging;
using Psi.ServiceDiscovery.Abstractions;
using Psi.ServiceDiscovery.Models;

namespace Psi.ServiceDiscovery;

public abstract class ServiceFinder : IServiceFinder
{
    protected readonly ILogger Logger;

    protected ServiceFinder(ILogger logger)
    {
        Logger = logger;
    }

    public abstract Task<List<ServiceAddress>> QueryService(Type serviceType);
}
