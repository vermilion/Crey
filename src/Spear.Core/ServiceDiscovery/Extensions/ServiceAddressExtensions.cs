using System.Net;
using System.Text.RegularExpressions;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.ServiceDiscovery.Extensions;

public static class ServiceAddressExtensions
{
    public static EndPoint ToEndpoint(this ServiceAddress address)
    {
        var ipRegex = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

        if (Regex.IsMatch(address.Host, ipRegex))
            return new IPEndPoint(IPAddress.Parse(address.Host), address.Port);

        return new DnsEndPoint(address.Host, address.Port);
    }

    public static ServiceAddress Random(this IList<ServiceAddress> services)
    {
        if (services == null || !services.Any()) return null;
        if (services.Count == 1) return services.First();

        // order by weight
        var sum = services.Sum(t => t.Weight);
        var rand = RandomHelper.Random().NextDouble() * sum;
        var tempWeight = 0D;

        foreach (var service in services)
        {
            tempWeight += service.Weight;
            if (rand <= tempWeight)
                return service;
        }

        return services.RandomSort().First();
    }
}
