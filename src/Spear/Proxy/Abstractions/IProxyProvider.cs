using System.Reflection;

namespace Spear.ProxyGenerator.Abstractions;

public interface IProxyProvider
{
    object Invoke(MethodInfo method, IDictionary<string, object> parameters);

    Task InvokeAsync(MethodInfo method, IDictionary<string, object> parameters);

    Task<T> InvokeAsync<T>(MethodInfo method, IDictionary<string, object> parameters);
}
