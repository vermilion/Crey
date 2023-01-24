using Microsoft.Extensions.DependencyInjection;

namespace Crey.Discovery;

public static class ServiceCollectionExtensions
{
    internal static void AddServiceDiscoveryServices(this IServiceCollection services)
    {
        services.AddHostedService<ServiceRegisterBackroundService>();
    }
}
