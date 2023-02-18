using System.Reflection;

namespace Crey.Service;

public delegate Task<object> ServiceEntryDelegate(IServiceProvider provider, IDictionary<string, object> parameters);

public interface IServiceEntryFactory
{
    IEnumerable<Assembly> GetContracts();

    ServiceEntryDelegate? Find(string serviceId);
}
