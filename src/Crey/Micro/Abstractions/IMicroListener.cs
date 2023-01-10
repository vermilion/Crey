using Crey.Message;
using Crey.ServiceDiscovery.Models;

namespace Crey.Micro;

public interface IMicroListener : IMessageListener
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
