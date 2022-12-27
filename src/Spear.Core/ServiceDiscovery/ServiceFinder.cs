using Microsoft.Extensions.Logging;
using Spear.Core.ServiceDiscovery.Abstractions;

namespace Spear.Core.ServiceDiscovery;

public abstract class ServiceFinder : IServiceFinder
{
    protected readonly ILogger Logger;

    protected ServiceFinder(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary> 查询服务 </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public abstract Task<List<ServiceAddress>> QueryService(Type serviceType);
}
