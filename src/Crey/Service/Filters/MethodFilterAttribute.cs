namespace Crey.Service;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MethodFilterAttribute : Attribute
{
    public Type Type { get; set; }
}


[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MethodFilterAttribute<T> : MethodFilterAttribute
    where T : IServiceMethodFilter
{
    public MethodFilterAttribute()
    {
        Type = typeof(T);
    }
}
