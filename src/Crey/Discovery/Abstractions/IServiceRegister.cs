using System.Reflection;

namespace Crey.Discovery;

/// <summary>
/// Allows to manage server services
/// </summary>
public interface IServiceRegister
{
    /// <summary>
    /// Registers services from assemblies at specified address
    /// </summary>
    /// <param name="assemblyList">List of found assemblies</param>
    /// <param name="serverAddress">Address</param>
    /// <returns><see cref="Task"/></returns>
    Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress);

    /// <summary>
    /// Deregisters all services
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    Task Deregister();
}
