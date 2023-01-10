using Crey.Extensions;
using Crey.Micro;

namespace Crey.Discovery.StaticRouter;

public class StaticListOptions
{
    internal Dictionary<string, ServiceAddress[]> Services { get; set; } = new();

    public StaticListOptions Set<T>(ServiceAddress[] addresses)
        where T : class, IMicroService
    {
        var contractName = typeof(T).Assembly.ServiceName();

        Services.Add(contractName, addresses);
        return this;
    }
}
