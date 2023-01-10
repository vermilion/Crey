using Crey.ServiceDiscovery.Models;

namespace Crey.Micro;

public interface IMicroHost
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
