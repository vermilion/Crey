using Crey.Micro.Abstractions;
using Crey.Proxy.Abstractions;
using Crey.Proxy.ProxyGenerator;

namespace Crey.Proxy;

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
