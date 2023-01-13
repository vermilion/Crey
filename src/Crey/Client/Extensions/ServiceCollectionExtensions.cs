using Microsoft.Extensions.DependencyInjection;

namespace Crey.Client;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Client services to <see cref="IServiceCollection"/>
    /// </summary>
    public static IServiceCollection AddClientServices(this IServiceCollection services)
    {
        services.AddSingleton<IClientMethodExecutor, ClientMethodExecutor>();

        return services;
    }
}
