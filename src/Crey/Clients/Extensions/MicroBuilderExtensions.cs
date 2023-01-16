using Crey.Builder;
using Crey.Codec.MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Crey.Clients;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds client-related services
    /// </summary>
    /// <param name="builder">Builder instance <see cref="IMicroBuilder"/></param>
    /// <param name="builderAction">Configure action</param>
    /// <returns>Fluent for <see cref="IMicroBuilder"/></returns>
    public static IMicroBuilder AddMicroClient(this IMicroBuilder builder, Action<IMicroClientBuilder>? builderAction = null)
    {
        var services = builder.Services;

        builderAction?.Invoke(builder);

        services.AddBaseServices();

        services.AddProxyServices();
        services.AddClientServices();

        return builder;
    }
}
