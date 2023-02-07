using Microsoft.Extensions.DependencyInjection;

namespace Crey.Exceptions;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds exception services
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddExceptionHandlingServices(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionConverter, ExceptionConverter>();

        return services;
    }
}
