using System.Reflection;

namespace Crey.Micro;

public delegate Task<object> MicroEntryDelegate(IServiceProvider provider, IDictionary<string, object> parameters);

public interface IMicroEntryFactory
{
    IEnumerable<Assembly> GetContracts();

    MicroEntryDelegate? Find(string serviceId);
}
