using System.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Discovery;

public static class MicroBuilderExtensions
{
    internal static void AddServiceDiscoveryServices(this IMicroBuilder builder)
    {
        var services = builder.Services;

        services.Configure<DiscoveryOptions>(builder.ConfigurationSection.GetSection("discovery"));

        services.AddHostedService<ServiceRegisterBackroundService>();
        services.AddHostedService<ServiceMonitorBackroundService>();
    }
}
