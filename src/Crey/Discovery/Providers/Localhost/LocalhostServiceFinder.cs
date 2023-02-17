using System.Net;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace Crey.Discovery.Localhost;

internal class LocalhostServiceFinder : IServiceFinder, IServiceRegister
{
    private readonly List<ServiceAddress> _serviceRegistry;

    public LocalhostServiceFinder(IOptions<LocalhostDiscoveryOptions> options)
    {
        _serviceRegistry = new()
        {
             new ServiceAddress(IPAddress.Loopback.ToString(), options.Value.Port)
        };
    }

    public Task Deregister()
    {
        return Task.CompletedTask;
    }

    public Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress)
    {
        return Task.CompletedTask;
    }

    public Task<List<ServiceAddress>> QueryServices(Type serviceType)
    {
        return Task.FromResult(_serviceRegistry);
    }
}
