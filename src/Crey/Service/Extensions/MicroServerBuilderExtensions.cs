using Microsoft.Extensions.DependencyInjection;
using Crey.Builder;
using Crey.Contracts;

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

    /// <summary>
    /// Adds method filter to collection
    /// </summary>
    /// <typeparam name="TFilter">Filter type</typeparam>
    /// <param name="builder">Fluent for <see cref="IMicroServerBuilder"/></param>
    public static void AddMethodFilter<TFilter>(this IMicroServerBuilder builder)
        where TFilter : class, IServiceMethodFilter
    {
        builder.Services.AddScoped<IServiceMethodFilter, TFilter>();
        builder.Services.AddScoped<TFilter>();
    }
}
