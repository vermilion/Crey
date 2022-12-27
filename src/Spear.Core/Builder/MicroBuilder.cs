using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Builder.Abstractions;

namespace Spear.Core.Builder;

public class MicroBuilder : IMicroBuilder
{
    public MicroBuilder(IConfiguration configuration, IServiceCollection services)
    {
        Configuration = configuration;
        Services = services;
    }

    public IConfiguration Configuration { get; }

    public IServiceCollection Services { get; }
}
