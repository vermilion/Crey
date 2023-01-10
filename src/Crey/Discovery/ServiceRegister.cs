using System.Reflection;

namespace Crey.Discovery;

public abstract class ServiceRegister : IServiceRegister
{
    public abstract Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress);

    public abstract Task Deregister();
}
