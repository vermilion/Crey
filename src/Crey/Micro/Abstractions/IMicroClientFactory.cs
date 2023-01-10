using Crey.ServiceDiscovery.Models;

namespace Crey.Micro;

public interface IMicroClientFactory
{
    Task<IMicroClient> CreateClient(ServiceAddress serviceAddress);
}
