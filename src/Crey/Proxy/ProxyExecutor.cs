using System.Reflection;

namespace Crey.Proxy;

internal class ProxyExecutor : IProxyExecutor
{
    private readonly IProxyProvider _proxyProvider;

    public ProxyExecutor(IProxyProvider provider)
    {
        _proxyProvider = provider;
    }

    public object Invoke(MethodInfo method, object[] args)
    {
        var parameters = GetParameters(method, args);
        return _proxyProvider.Invoke(method, parameters);
    }

    public Task InvokeAsync(MethodInfo method, object[] args)
    {
        var parameters = GetParameters(method, args);
        return _proxyProvider.InvokeAsync(method, parameters);
    }

    public Task<T> InvokeAsync<T>(MethodInfo method, object[] args)
    {
        var parameters = GetParameters(method, args);
        return _proxyProvider.InvokeAsync<T>(method, parameters);
    }

    private static IDictionary<string, object> GetParameters(MethodBase method, IReadOnlyList<object> args)
    {
        var dict = new Dictionary<string, object>();
        var parameters = method.GetParameters();
        if (!parameters.Any())
            return dict;

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            dict.Add(parameter.Name, args[i]);
        }

        return dict;
    }
}
