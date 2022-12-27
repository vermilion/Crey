using Spear.Core.ServiceDiscovery.Models;

namespace Spear.Core.ServiceDiscovery.StaticRouter.Options;

public class StaticRouterOptions
{
    public Dictionary<string, ServiceAddress[]> Services { get; set; } = new ();
}
