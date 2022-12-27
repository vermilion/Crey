namespace Spear.Core.ServiceDiscovery.Abstractions;

/// <summary> 服务探测器 </summary>
public interface IServiceFinder
{
    /// <summary> 服务发现 </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    Task<List<ServiceAddress>> QueryService(Type serviceType);
}
