using Spear.Core.ServiceDiscovery;

namespace Spear.Core.Micro.Abstractions
{
    /// <summary> 微服务主机 </summary>
    public interface IMicroHost
    {
        /// <summary> 启动服务 </summary>
        /// <param name="serviceAddress"></param>
        /// <returns></returns>
        Task Start(ServiceAddress serviceAddress);

        /// <summary> 停止服务 </summary>
        /// <returns></returns>
        Task Stop();
    }
}
