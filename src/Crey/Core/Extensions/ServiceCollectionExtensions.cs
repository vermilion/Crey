using Crey.Codec.MessagePack;
using Crey.Transport.TCP;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Core;

internal static class ServiceCollectionExtensions
{
    internal static void AddBaseServices(this IServiceCollection services)
    {
        services.AddSingleton(services);

        services.AddExceptionHandlingServices();
        services.AddMessagePackCodec();
        services.AddTcpProtocol();
    }
}
