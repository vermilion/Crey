using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Builder.Abstractions;
using Spear.Core.ServiceDiscovery.Abstractions;
using Spear.Core.ServiceDiscovery.StaticRouter.Options;

namespace Spear.Core.ServiceDiscovery.StaticRouter.Extensions;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds Static discovery as default
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <param name="routerAction">Action to configure options</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddStaticServiceDiscovery(this IMicroBuilder builder, Action<StaticRouterOptions> routerAction = null)
    {
        var config = builder.Configuration.GetSection("micro:router").Get<StaticRouterOptions>();
        routerAction?.Invoke(config);

        //builder.Services.Configure<DefaultRouterOptions>();

        builder.Services.AddSingleton<IServiceRegister, StaticServiceRouter>();
        builder.Services.AddSingleton<IServiceFinder, StaticServiceRouter>();
        return builder;
    }
}
