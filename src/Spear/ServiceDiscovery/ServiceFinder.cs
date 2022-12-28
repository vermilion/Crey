using Microsoft.Extensions.Logging;
using Spear.Core.ServiceDiscovery.Abstractions;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.ServiceDiscovery;

public abstract class ServiceFinder : IServiceFinder
{
    protected readonly ILogger Logger;

    protected ServiceFinder(ILogger logger)
    {
        Logger = logger;
    }

    public abstract Task<List<ServiceAddress>> QueryService(Type serviceType);
}
