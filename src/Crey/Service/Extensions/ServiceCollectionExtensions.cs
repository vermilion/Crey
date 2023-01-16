using Microsoft.Extensions.DependencyInjection;

namespace Crey.Service;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds service related services
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddServiceServices(this IServiceCollection services)
    {
        services.AddSingleton<IServiceHost, ServiceHost>();
        services.AddScoped<IServiceMethodExecutor, ServiceMethodExecutor>();

        return services;
    }
}
