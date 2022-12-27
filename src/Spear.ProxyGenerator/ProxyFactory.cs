using Spear.ProxyGenerator.Abstractions;
using Spear.ProxyGenerator.Proxy;

namespace Spear.ProxyGenerator;

public class ProxyFactory : IProxyFactory
{
    private readonly IResolver _resolver;
    private readonly IProxyProvider _proxyProvider;
    private readonly AsyncProxyGenerator _proxyGenerator;

    public ProxyFactory(IResolver resolver, IProxyProvider proxyProvider, AsyncProxyGenerator proxyGenerator)
    {
        _resolver = resolver;
        _proxyProvider = proxyProvider;
        _proxyGenerator = proxyGenerator;
    }

    public T Create<T>(object key = null)
        where T : class
    {
        return (T)Create(typeof(T), key);
    }

    private object Create(Type type, object key = null)
    {
        var instance = _resolver.Resolve(type, key);
        if (instance != null) return instance;

        instance = _proxyGenerator.CreateProxy(type, typeof(ProxyExecutor), _proxyProvider, key);
        _resolver.Register(type, instance, key);
        return instance;
    }
}
