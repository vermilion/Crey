using System.Reflection;

namespace Spear.Core.ServiceDiscovery.Abstractions;

/// <summary> 服务注册器 </summary>
public interface IServiceRegister
{
    /// <summary> 注册服务 </summary>
    Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress);

    /// <summary> 注销服务 </summary>
    Task Deregister();
}
