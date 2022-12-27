using System.Reflection;
using Spear.Core.Micro.Models;

namespace Spear.Core.Micro.Abstractions;

public interface IMicroEntryFactory
{
    List<Type> Services { get; }

    IEnumerable<Assembly> GetContracts();

    MicroEntry? Find(string serviceId);
}
