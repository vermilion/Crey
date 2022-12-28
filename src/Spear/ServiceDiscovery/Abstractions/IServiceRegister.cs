using System.Reflection;
using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.ServiceDiscovery.Abstractions;

public interface IServiceRegister
{
    Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress);

    Task Deregister();
}
