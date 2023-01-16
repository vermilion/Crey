using Crey.Contracts;
using Crey.Extensions;

namespace Crey.Discovery.StaticList;

public class StaticListOptions
{
    internal Dictionary<string, ServiceAddress[]> Services { get; set; } = new();

    /// <summary>
    /// Sets array of addresses for given service contract
    /// </summary>
    /// <typeparam name="T">Service contract type</typeparam>
    /// <param name="addresses">Static list of available addresses</param>
    /// <returns>Fluent for <see cref="StaticListOptions"/></returns>
    public StaticListOptions Set<T>(ServiceAddress[] addresses)
        where T : class, IMicroService
    {
        var contractName = typeof(T).Assembly.ServiceName();

        Services.Add(contractName, addresses);
        return this;
    }
}
