using Microsoft.Extensions.Logging;

namespace Crey.Discovery;

public abstract class ServiceFinder : IServiceFinder
{
    protected readonly ILogger Logger;

    protected ServiceFinder(ILogger logger)
    {
        Logger = logger;
    }

    public abstract Task<List<ServiceAddress>> QueryService(Type serviceType);
}
