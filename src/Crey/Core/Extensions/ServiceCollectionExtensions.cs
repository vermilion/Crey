using Crey.Codec.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Core;

public static class ServiceCollectionExtensions
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton(services);

        services.AddExceptionHandlingServices();
        services.AddMessagePackCodec();
        //services.AddTcpProtocol();
    }
}
