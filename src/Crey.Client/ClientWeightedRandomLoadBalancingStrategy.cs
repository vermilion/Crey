using System.Security.Cryptography;

namespace Crey.ClientSide;

internal class ClientWeightedRandomLoadBalancingStrategy : IClientLoadBalancingStrategy
{
    public Task<ServiceAddress?> GetNextService(IEnumerable<ServiceAddress> services)
    {
        var service = Random(services);
        return Task.FromResult(service);
    }

    private ServiceAddress? Random(IEnumerable<ServiceAddress> services)
    {
        if (services == null || !services.Any())
            return null;

        if (services.Count() == 1)
            return services.First();

        // order by weight
        var sum = services.Sum(t => t.Weight);
        var rand = Random().NextDouble() * sum;
        var tempWeight = 0D;

        foreach (var service in services)
        {
            tempWeight += service.Weight;
            if (rand <= tempWeight)
                return service;
        }

        return services.OrderBy(t => Random().Next()).First();
    }

    private static Random Random()
    {
        var bytes = new byte[4];

        using var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(bytes);

        var seed = BitConverter.ToInt32(bytes, 0);
        var tick = DateTime.Now.Ticks + seed;
        return new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
    }
}
