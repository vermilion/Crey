using System.Reflection;

namespace Spear.Core.Micro.Models;

public class MicroEntry
{
    public MethodInfo Method { get; }
    public Func<IDictionary<string, object>, Task<object>> Invoke { get; set; }
    public bool IsTask { get; }
    public bool IsLongRunning { get; }
    public ParameterInfo[] Parameters { get; }
    
    public MicroEntry(MethodInfo method)
    {
        Method = method;
        IsTask = Method.ReturnType == typeof(Task) || Method.ReturnType == typeof(Task<>);
        Parameters = method.GetParameters();
    }
}
