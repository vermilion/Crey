using Crey.Codec.MessagePack;
using Crey.Exceptions;
using Crey.Transport.TCP;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Core;

public static class ServiceCollectionExtensions
{
    internal static void AddBaseServices(this IServiceCollection services)
    {
        services.AddSingleton(services);

        services.AddExceptionHandlingServices();
        services.AddMessagePackCodec();
        services.AddTcpProtocol();
    }
}
