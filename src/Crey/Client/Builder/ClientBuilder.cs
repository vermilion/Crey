using Crey.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Client;

/// <summary>
/// Default client builder
/// </summary>
public static class ClientBuilder
{
    /// <summary>
    /// Creates client with defaults applying configuration expression
    /// </summary>
    /// <param name="configureAction">Configure action</param>
    /// <returns>Builder <see cref="BuildableClient"/></returns>
    public static BuildableClient Create(Action<MicroBuilder> configureAction)
    {
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.Build();
        var services = new ServiceCollection();
        services.AddLogging();

        var builder = new MicroBuilder(configuration, services);

        configureAction(builder);

        return new BuildableClient(builder.Services.BuildServiceProvider());
    }

    /// <summary>
    /// Creates client using existing configured <see cref="IServiceProvider"/>
    /// </summary>
    /// <param name="provider"><see cref="IServiceProvider"/></param>
    /// <returns>Builder <see cref="BuildableClient"/></returns>
    public static BuildableClient Create(IServiceProvider provider)
    {
        return new BuildableClient(provider);
    }
}

public class BuildableClient
{
    protected readonly IServiceProvider _provider;

    public BuildableClient(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Gets client resolve factory from <see cref="IServiceProvider"/>
    /// </summary>
    /// <returns>Factory <see cref="IProxyFactory"/></returns>
    public IProxyFactory CreateProxyFactory()
    {
        return _provider.GetRequiredService<IProxyFactory>();
    }
}
