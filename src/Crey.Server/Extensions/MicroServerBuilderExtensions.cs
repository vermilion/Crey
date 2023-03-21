using Microsoft.Extensions.DependencyInjection;

namespace Crey.Service;

public static class MicroServerBuilderExtensions
{
    /// <summary>
    /// Adds contract to collection
    /// </summary>
    /// <typeparam name="T">Contract type</typeparam>
    /// <typeparam name="TImplementation">Contract implementation</typeparam>
    /// <param name="builder">Fluent for <see cref="IMicroServerBuilder"/></param>
    public static void AddContract<T, TImplementation>(this IMicroServerBuilder builder)
        where T : class, IMicroService
        where TImplementation : class, T
    {
        builder.Services.AddScoped<T, TImplementation>();
    }

    /// <summary>
    /// Adds middleware to collection
    /// </summary>
    /// <typeparam name="TMiddleware">Middleware type</typeparam>
    /// <param name="builder">Fluent for <see cref="IMicroServerBuilder"/></param>
    public static void AddMiddleware<TMiddleware>(this IMicroServerBuilder builder)
        where TMiddleware : class, IServiceMiddleware
    {
        builder.Services.AddScoped<IServiceMiddleware, TMiddleware>();
    }
}
