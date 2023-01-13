using System.Reflection;
using Crey.Client;

namespace Crey.Proxy;

public class ProxyProvider : IProxyProvider
{
    private readonly IClientMethodExecutor _clientMethodExecutor;

    public ProxyProvider(IClientMethodExecutor clientMethodExecutor)
    {
        _clientMethodExecutor = clientMethodExecutor;
    }

    public object Invoke(MethodInfo method, IDictionary<string, object> parameters)
    {
        var result = _clientMethodExecutor.Execute(method, parameters).ConfigureAwait(false).GetAwaiter().GetResult();
        return result.Content;
    }

    public Task InvokeAsync(MethodInfo method, IDictionary<string, object> parameters)
    {
        return _clientMethodExecutor.Execute(method, parameters);
    }

    public async Task<T> InvokeAsync<T>(MethodInfo method, IDictionary<string, object> parameters)
    {
        var result = await _clientMethodExecutor.Execute(method, parameters);
        return (T)result.Content;
    }
}
