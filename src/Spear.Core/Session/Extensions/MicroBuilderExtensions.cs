using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Builder.Abstractions;
using Spear.Core.Session.Abstractions;

namespace Spear.Core.Session.Extensions;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds session provider
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddSession(this IMicroBuilder builder)
    {
        builder.AddSession<DefaultPrincipalAccessor>();
        return builder;
    }

    /// <summary>
    /// Adds session provider
    /// </summary>
    /// <typeparam name="T">Provider type</typeparam>
    /// <param name="builder">Builder instance</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddSession<T>(this IMicroBuilder builder)
        where T : class, IPrincipalAccessor
    {
        builder.Services.AddScoped<IPrincipalAccessor, T>();
        builder.Services.AddScoped<IMicroSession, ClaimMicroSession>();
        return builder;
    }
}
