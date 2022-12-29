using System.Reflection;
using Psi.ServiceDiscovery.Abstractions;
using Psi.ServiceDiscovery.Models;

namespace Psi.ServiceDiscovery;

public abstract class ServiceRegister : IServiceRegister
{
    public abstract Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress);

    public abstract Task Deregister();
}
