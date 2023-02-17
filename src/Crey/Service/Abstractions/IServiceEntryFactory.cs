using System.Reflection;

namespace Crey.Service;

public interface IServiceEntryFactory
{
    IEnumerable<Assembly> GetContracts();

    ServiceEntryInfo? Find(string serviceId);
}
