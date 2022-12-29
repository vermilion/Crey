using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Psi.Builder.Abstractions;

/// <summary>
/// Base builder interface
/// </summary>
public interface IBuilder
{
    /// <summary>
    /// Inner configuration section
    /// </summary>
    IConfigurationSection ConfigurationSection { get; }

    /// <summary>
    /// Builder-wide service collection
    /// </summary>
    IServiceCollection Services { get; }
}
