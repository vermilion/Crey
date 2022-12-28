using Microsoft.Extensions.DependencyInjection;
using Psi.Builder.Abstractions;
using Psi.Micro.Abstractions;

namespace Psi.Micro.Extensions;

public static class MicroServerBuilderExtensions
{
    public static void AddContract<T, TImplementation>(this IMicroServerBuilder builder)
        where T : class, IMicroService
        where TImplementation : class, T
    {
        builder.Services.AddScoped<T, TImplementation>();
    }
}
