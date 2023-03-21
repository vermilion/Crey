using System.Reflection;
using Crey.ClientSide;

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

    public async Task InvokeAsync(MethodInfo method, IDictionary<string, object> parameters)
    {
        await _clientMethodExecutor.Execute(method, parameters).ConfigureAwait(false);
    }

    public async Task<T> InvokeAsync<T>(MethodInfo method, IDictionary<string, object> parameters)
    {
        var result = await _clientMethodExecutor.Execute(method, parameters).ConfigureAwait(false);
        return (T)result.Content;
    }
}
