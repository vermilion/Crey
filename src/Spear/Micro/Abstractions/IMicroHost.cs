using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.Micro.Abstractions;

public interface IMicroHost
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
