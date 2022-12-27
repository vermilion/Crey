using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Builder.Abstractions;
using Spear.Core.Micro.Abstractions;

namespace Spear.Protocol.Tcp.Extensions;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Registers DotNetty TCP transport protocol
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
