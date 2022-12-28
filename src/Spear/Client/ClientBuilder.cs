using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Builder;
using Spear.ProxyGenerator.Abstractions;

namespace Spear.Client;

public static class ClientBuilder
{
    public static SpearBuildableClient Create(Action<MicroBuilder> configureAction)
    {
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.Build();
        var services = new ServiceCollection();
        services.AddLogging();

        var builder = new MicroBuilder(configuration, services);

        configureAction(builder);

        return new SpearBuildableClient(builder.Services.BuildServiceProvider());
    }

    public static SpearBuildableClient Create(IServiceProvider provider)
    {
        return new SpearBuildableClient(provider);
    }
}

public class SpearBuildableClient
{
    protected readonly IServiceProvider _provider;

    public SpearBuildableClient(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IProxyFactory CreateProxyFactory()
    {
        return _provider.GetRequiredService<IProxyFactory>();
    }
}
