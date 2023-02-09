using Microsoft.Extensions.DependencyInjection;

namespace Crey.ClientSide;

public static class MicroClientBuilderExtensions
{
    /// <summary>
    /// Adds middleware to collection
    /// </summary>
    /// <typeparam name="TMiddleware">Middleware type</typeparam>
    /// <param name="builder">Fluent for <see cref="IMicroClientBuilder"/></param>
    public static void AddMiddleware<TMiddleware>(this IMicroClientBuilder builder)
        where TMiddleware : class, IClientMiddleware
    {
        builder.Services.AddScoped<IClientMiddleware, TMiddleware>();
    }
}
