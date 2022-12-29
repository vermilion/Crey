using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Psi.Builder.Abstractions;

namespace Psi.Builder;

public class MicroBuilder : IMicroBuilder
{
    public MicroBuilder(IConfiguration configuration, IServiceCollection services)
    {
        ConfigurationSection = configuration.GetSection("micro");
        Services = services;
    }

    public IConfigurationSection ConfigurationSection { get; }

    public IServiceCollection Services { get; }
}
