using Microsoft.Extensions.DependencyInjection;
using Crey.Builder.Abstractions;
using Crey.Micro.Abstractions;

namespace Crey.Protocol.Tcp.Extensions;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Registers DotNetty TCP transport protocol as default
    /// </summary>
    /// <param name="builder">Builder instance</param>
    /// <returns>Builder instance</returns>
    public static IMicroBuilder AddTcpProtocol(this IMicroBuilder builder)
    {
        builder.Services.AddSingleton<IMicroListener, DotNettyMicroListener>();
        builder.Services.AddSingleton<IMicroClientFactory, DotNettyClientFactory>();

        return builder;
    }
}
