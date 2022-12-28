using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Builder.Abstractions;
using Spear.Core.Micro.Abstractions;

namespace Spear.Micro.Extensions;

public static class MicroServerBuilderExtensions
{
    public static void AddContract<T, TImplementation>(this IMicroServerBuilder builder)
        where T : class, IMicroService
        where TImplementation : class, T
    {
        builder.Services.AddScoped<T, TImplementation>();
    }
}
