using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.Micro.Abstractions;

public interface IMicroClientFactory
{
    Task<IMicroClient> CreateClient(ServiceAddress serviceAddress);
}
