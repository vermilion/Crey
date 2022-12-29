using Psi.Message;
using Psi.Micro.Abstractions;
using Psi.ServiceDiscovery.Models;

namespace Psi.Micro;

public abstract class MicroListener : MessageListener, IMicroListener
{
    public abstract Task Start(ServiceAddress serviceAddress);

    public abstract Task Stop();
}
