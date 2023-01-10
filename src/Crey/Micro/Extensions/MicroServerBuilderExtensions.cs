using Microsoft.Extensions.DependencyInjection;
using Crey.Builder;

namespace Crey.Micro;

public static class MicroServerBuilderExtensions
{
    public static void AddContract<T, TImplementation>(this IMicroServerBuilder builder)
        where T : class, IMicroService
        where TImplementation : class, T
    {
        builder.Services.AddScoped<T, TImplementation>();
    }
}
