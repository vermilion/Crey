using System.Reflection;

namespace Crey.Proxy;

public interface IProxyExecutor
{
    object Invoke(MethodInfo method, object[] args);
    Task InvokeAsync(MethodInfo method, object[] args);
    Task<T> InvokeAsync<T>(MethodInfo method, object[] args);
}