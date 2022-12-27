using System.Reflection;
using Spear.Core.ServiceDiscovery.Abstractions;

namespace Spear.Core.ServiceDiscovery;

public abstract class ServiceRegister : IServiceRegister
{
    public abstract Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress);

    public abstract Task Deregister();
}
