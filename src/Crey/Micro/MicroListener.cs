using Crey.Message;
using Crey.Discovery;

namespace Crey.Micro;

public abstract class MicroListener : MessageListener, IMicroListener
{
    public abstract Task Start(ServiceAddress serviceAddress);

    public abstract Task Stop();
}
