using Microsoft.Extensions.DependencyInjection;

namespace Crey.Discovery;

public static class MicroBuilderExtensions
{
    internal static void AddServiceDiscoveryServices(this IMicroBuilder builder)
    {
        var services = builder.Services;

        services.AddHostedService<ServiceRegisterBackroundService>();
    }
}
