using Castle.DynamicProxy;

namespace Crey.Proxy;

internal class ProxyFactory : IProxyFactory
{
    private readonly ProxyInterceptor _interceptor;
    private readonly ProxyGenerator _proxyGenerator = new();

    public ProxyFactory(ProxyInterceptor interceptor)
    {
        _interceptor = interceptor;
    }

    public T Proxy<T>()
        where T : class, IMicroService
    {
        return _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(_interceptor.ToInterceptor());
    }
}
