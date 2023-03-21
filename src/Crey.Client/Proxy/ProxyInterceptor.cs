using Castle.DynamicProxy;

namespace Crey.Proxy;

internal class ProxyInterceptor : IAsyncInterceptor
{
    private readonly IProxyExecutor _proxyExecutor;

    public ProxyInterceptor(IProxyExecutor proxyExecutor)
    {
        _proxyExecutor = proxyExecutor;
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
