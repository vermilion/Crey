namespace Crey.Service;

public delegate Task<object> MicroEntryDelegate(IServiceProvider provider, IDictionary<string, object> parameters);

public class ServiceEntryInfo
{
    public ServiceEntryInfo(MicroEntryDelegate @delegate)
    {
        Delegate = @delegate;
    }

    public MicroEntryDelegate Delegate { get; }
    public List<Type> MethodFilters { get; set; } = new();
}