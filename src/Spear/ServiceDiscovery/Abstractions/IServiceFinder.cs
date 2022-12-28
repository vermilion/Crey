using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.ServiceDiscovery.Abstractions;

public interface IServiceFinder
{
    Task<List<ServiceAddress>> QueryService(Type serviceType);
}
