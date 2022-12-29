using Psi.Extensions;
using Psi.Micro.Abstractions;
using Psi.ServiceDiscovery.Models;

namespace Psi.ServiceDiscovery.StaticRouter.Options;

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
