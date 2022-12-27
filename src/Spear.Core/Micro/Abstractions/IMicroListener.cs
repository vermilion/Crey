using System.Threading.Tasks;
using Spear.Core.Message.Abstractions;
using Spear.Core.ServiceDiscovery;

namespace Spear.Core.Micro.Abstractions
{
    /// <summary> 微服务监听者 </summary>
    public interface IMicroListener : IMessageListener
    {
        /// <summary> 启动监听 </summary>
        /// <param name="serviceAddress"></param>
        /// <returns></returns>
        Task Start(ServiceAddress serviceAddress);

        /// <summary> 停止监听 </summary>
        /// <returns></returns>
        Task Stop();
    }
}
