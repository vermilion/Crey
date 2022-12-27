using System.Reflection;

namespace Spear.ProxyGenerator.Abstractions;

public interface IProxyProvider
{
    object Invoke(MethodInfo method, IDictionary<string, object> parameters, object key = null);

    Task InvokeAsync(MethodInfo method, IDictionary<string, object> parameters, object key = null);

    Task<T> InvokeAsync<T>(MethodInfo method, IDictionary<string, object> parameters, object key = null);
}
