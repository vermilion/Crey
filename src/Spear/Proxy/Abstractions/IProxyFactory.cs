using Spear.Core.Micro.Abstractions;

namespace Spear.ProxyGenerator.Abstractions;

/// <summary>
/// Proxy factory generator
/// </summary>
public interface IProxyFactory
{
    /// <summary>
    /// Allows to create Dispatch proxy for given interface type
    /// </summary>
    /// <typeparam name="T">Interface type</typeparam>
    /// <returns>Generated Dispatch Proxy</returns>
    T Create<T>() where T : class, IMicroService;
}
