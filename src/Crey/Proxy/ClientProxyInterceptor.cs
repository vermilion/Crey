using Castle.DynamicProxy;

namespace Crey.Proxy;

public class ClientProxyInterceptor : IAsyncInterceptor
{
    private readonly ProxyExecutor _proxyExecutor;

    public ClientProxyInterceptor(IProxyProvider proxyProvider)
    {
        _proxyExecutor = new ProxyExecutor(proxyProvider);
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = _proxyExecutor.InvokeAsync(invocation.Method, invocation.Arguments);
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = _proxyExecutor.InvokeAsync<TResult>(invocation.Method, invocation.Arguments);
    }

    public void InterceptSynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = _proxyExecutor.Invoke(invocation.Method, invocation.Arguments);
    }
}
