using Spear.Core.Message.Abstractions;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.Micro.Abstractions;

public interface IMicroListener : IMessageListener
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
