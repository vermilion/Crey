using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Spear.Core.Builder.Abstractions;

public interface IBuilder
{
    IConfigurationSection ConfigurationSection { get; }
    IServiceCollection Services { get; }
}
