using Castle.DynamicProxy;
using Crey.Micro.Abstractions;
using Crey.Proxy.Abstractions;

namespace Crey.Proxy;

public class ProxyFactory : IProxyFactory
{
    private readonly ClientProxyInterceptor _interceptor;
    private readonly ProxyGenerator _proxyGenerator = new();

    public ProxyFactory(ClientProxyInterceptor interceptor)
    {
        _interceptor = interceptor;
    }

    public T Create<T>()
        where T : class, IMicroService
    {
        return _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(_interceptor.ToInterceptor());
    }
}
