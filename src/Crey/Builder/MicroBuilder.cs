using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Builder;

public class MicroBuilder : IMicroBuilder
{
    public MicroBuilder(IConfiguration configuration, IServiceCollection services)
    {
        ConfigurationSection = configuration.GetSection("Micro");
        Services = services;
    }

    public IConfigurationSection ConfigurationSection { get; }

    public IServiceCollection Services { get; }
}
