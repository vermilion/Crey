using Crey.Service;

namespace Crey.ClientSide;

public interface IClientLoadBalancingStrategy
{
    Task<ServiceAddress?> GetNextService(IEnumerable<ServiceAddress> services);
}