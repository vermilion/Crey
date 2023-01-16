using System.Reflection;

namespace Crey.Service;

public delegate Task<object> MicroEntryDelegate(IServiceProvider provider, IDictionary<string, object> parameters);

public interface IServiceEntryFactory
{
    IEnumerable<Assembly> GetContracts();

    MicroEntryDelegate? Find(string serviceId);
}
