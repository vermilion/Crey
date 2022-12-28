using Spear.Core.Message;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.Micro;

public abstract class MicroListener : MessageListener, IMicroListener
{
    public abstract Task Start(ServiceAddress serviceAddress);

    public abstract Task Stop();
}
