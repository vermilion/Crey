using Psi.Message.Abstractions;
using Psi.ServiceDiscovery.Models;

namespace Psi.Micro.Abstractions;

public interface IMicroListener : IMessageListener
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
