using System.Reflection;

namespace Spear.Core.Micro.Services;

public abstract class DServiceRegister : DServiceRoute, IServiceRegister
{
    public abstract Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress);

    public abstract Task Deregister();
}
