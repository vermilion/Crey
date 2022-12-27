namespace Spear.ProxyGenerator.Abstractions;

public interface IProxyFactory
{
    T Create<T>(object key = null) where T : class;
}
