using Psi.ServiceDiscovery.Models;

namespace Psi.Micro.Abstractions;

public interface IMicroClientFactory
{
    Task<IMicroClient> CreateClient(ServiceAddress serviceAddress);
}
