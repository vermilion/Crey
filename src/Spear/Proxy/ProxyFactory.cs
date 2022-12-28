using Spear.Core.Micro.Abstractions;
using Spear.ProxyGenerator.Abstractions;
using Spear.ProxyGenerator.Proxy;

namespace Spear.ProxyGenerator;

public class ProxyFactory : IProxyFactory
{
    private readonly IProxyProvider _proxyProvider;
    private readonly AsyncProxyGenerator _proxyGenerator;

    public ProxyFactory(IProxyProvider proxyProvider, AsyncProxyGenerator proxyGenerator)
    {
        _proxyProvider = proxyProvider;
        _proxyGenerator = proxyGenerator;
    }


    public T Create<T>()
        where T : class, IMicroService
    {
        return (T)_proxyGenerator.CreateProxy(typeof(T), typeof(ProxyExecutor), _proxyProvider);
    }
}
