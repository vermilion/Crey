using Spear.Core.Extensions;
using Spear.Core.ServiceDiscovery;

namespace Spear.Core.Options
{
    public class DefaultRouterOptions
    {
        /// <summary> 服务列表 </summary>
        public IDictionary<string, ServiceAddress[]> Services { get; set; } = new Dictionary<string, ServiceAddress[]>();
    }
    public class ServiceMode
    {
        public string Name { get; set; }
    }
}
