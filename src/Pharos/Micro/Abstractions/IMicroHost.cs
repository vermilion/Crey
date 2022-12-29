using Psi.ServiceDiscovery.Models;

namespace Psi.Micro.Abstractions;

public interface IMicroHost
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
