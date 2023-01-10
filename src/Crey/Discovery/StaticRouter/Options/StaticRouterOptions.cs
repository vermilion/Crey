using Crey.Extensions;
using Crey.Micro;
using Crey.Discovery;

namespace Crey.Discovery.StaticRouter;

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
