using Spear.Core.Micro.Abstractions;

namespace Spear.ProxyGenerator.Abstractions;

public interface IProxyFactory
{
    T Create<T>() where T : class, IMicroService;
}
