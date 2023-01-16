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
}
