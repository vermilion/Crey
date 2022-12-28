using Spear.Core.Extensions;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.ServiceDiscovery.StaticRouter.Options;

public class StaticRouterOptions
{
    internal Dictionary<string, ServiceAddress[]> Services { get; set; } = new();

    public StaticRouterOptions Set<T>(ServiceAddress[] addresses)
        where T: class, IMicroService
    {
        var contractName = typeof(T).Assembly.ServiceName();

        Services.Add(contractName, addresses);
        return this;
    }
}
