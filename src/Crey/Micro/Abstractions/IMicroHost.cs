using Crey.ServiceDiscovery.Models;

namespace Crey.Micro.Abstractions;

public interface IMicroHost
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
