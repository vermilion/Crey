using Crey.Message;
using Crey.Discovery;

namespace Crey.Micro;

public interface IMicroListener : IMessageListener
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
