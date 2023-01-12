using Crey.Discovery;
using Crey.Message;

namespace Crey.Micro;

public abstract class MicroListener : MessageListener, IMicroListener
{
    public abstract Task Start(ServiceAddress serviceAddress);

    public abstract Task Stop();
}
