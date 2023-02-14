using Microsoft.Extensions.Hosting;

namespace Crey.Builder;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Configures the host builder to host rpc services.
    /// </summary>
    /// <param name="hostBuilder">The host builder</param>
    /// <param name="configureDelegate">The delegate used to configure builder</param>
    /// <returns>The host builder</returns>
    public static IHostBuilder UseMicroServices(this IHostBuilder hostBuilder, Action<HostBuilderContext, IMicroBuilder> configureDelegate)
    {
        hostBuilder.ConfigureServices((context, services) =>
        {
            var builder = new MicroBuilder(context.Configuration, services);

            configureDelegate(context, builder);
        });

        return hostBuilder;
    }
}
