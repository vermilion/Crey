using System.Reflection;
using Crey.ServiceDiscovery.Abstractions;
using Crey.ServiceDiscovery.Models;

namespace Crey.ServiceDiscovery;

public abstract class ServiceRegister : IServiceRegister
{
    public abstract Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress);

    public abstract Task Deregister();
}
