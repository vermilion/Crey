namespace Spear.Core.Micro.Services;

/// <summary> 服务探测器 </summary>
public interface IServiceFinder
{
    /// <summary> 服务发现 </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    Task<List<ServiceAddress>> Find(Type serviceType);
}
