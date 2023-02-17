using Microsoft.Extensions.DependencyInjection;

namespace Crey.ClientSide;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds client-related services
    /// </summary>
    /// <param name="builder">Builder instance <see cref="IMicroBuilder"/></param>
    /// <param name="builderAction">Configure action</param>
    /// <returns>Fluent for <see cref="IMicroBuilder"/></returns>
    public static IMicroBuilder AddMicroClient(this IMicroBuilder builder, Action<IMicroClientBuilder>? builderAction = null)
    {
        var services = builder.Services;

        services.AddBaseServices();

        services.AddProxyServices();
        services.AddSingleton<IClientMethodExecutor, ClientMethodExecutor>();
        services.AddSingleton<IClientRetryPolicyProvider, ClientRetryPolicyProvider>();
        services.AddSingleton<IClientLoadBalancingStrategy, ClientWeightedRandomLoadBalancingStrategy>();

        builder.AddMiddleware<ClientCorrelationIdMiddleware>();

        builderAction?.Invoke(builder);

        return builder;
    }
}
