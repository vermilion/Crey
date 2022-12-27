using System.Threading.Tasks;
using Spear.Core.ServiceDiscovery;

namespace Spear.Core.Micro.Abstractions
{
    /// <summary> Spear客户端工厂 </summary>
    public interface IMicroClientFactory
    {
        /// <summary> 创建客户端 </summary>
        /// <param name="serviceAddress"></param>
        /// <returns></returns>
        Task<IMicroClient> CreateClient(ServiceAddress serviceAddress);
    }
}
