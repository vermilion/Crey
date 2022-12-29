using Crey.ServiceDiscovery.Models;

namespace Crey.Micro.Abstractions;

public interface IMicroClientFactory
{
    Task<IMicroClient> CreateClient(ServiceAddress serviceAddress);
}
