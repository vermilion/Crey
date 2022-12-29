using Crey.Message.Abstractions;
using Crey.ServiceDiscovery.Models;

namespace Crey.Micro.Abstractions;

public interface IMicroListener : IMessageListener
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
