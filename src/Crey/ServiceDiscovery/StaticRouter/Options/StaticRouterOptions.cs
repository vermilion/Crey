using Crey.Extensions;
using Crey.Micro.Abstractions;
using Crey.ServiceDiscovery.Models;

namespace Crey.ServiceDiscovery.StaticRouter.Options;

public class StaticRouterOptions
{
    internal Dictionary<string, ServiceAddress[]> Services { get; set; } = new();

    public StaticRouterOptions Set<T>(ServiceAddress[] addresses)
        where T : class, IMicroService
    {
        var contractName = typeof(T).Assembly.ServiceName();

        Services.Add(contractName, addresses);
        return this;
    }
}
