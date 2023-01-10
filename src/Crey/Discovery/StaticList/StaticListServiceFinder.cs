using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Crey.Extensions;
using Crey.Extensions.StringExtension;

namespace Crey.Discovery.StaticRouter;

public class StaticListServiceFinder : IServiceFinder, IServiceRegister
{
    private readonly Dictionary<string, List<ServiceAddress>> _serviceRegistry = new();
    private readonly ILogger<StaticListServiceFinder> _logger;

    public StaticListServiceFinder(
        IOptions<StaticListOptions> options,
        ILogger<StaticListServiceFinder> logger)
    {
        _logger = logger;

        RegisterFromConfig(options.Value);
    }

    private void RegisterFromConfig(StaticListOptions options)
    {
        if (options.Services.IsNullOrEmpty())
            return;

        foreach (var service in options.Services)
        {
            if (service.Key.IsNullOrEmpty() || service.Value.IsNullOrEmpty())
                continue;

            foreach (var address in service.Value)
            {
                Register(service.Key, address);
            }
        }
    }

    public Task Deregister()
    {
        _serviceRegistry.Clear();
        return Task.CompletedTask;
    }

    public Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress)
    {
        foreach (var assembly in assemblyList)
        {
            var serviceName = assembly.ServiceName();
            Register(serviceName, serverAddress);
        }

        return Task.CompletedTask;
    }

    public Task<List<ServiceAddress>> QueryService(Type serviceType)
    {
        var serviceName = serviceType.Assembly.ServiceName();
        if (!_serviceRegistry.TryGetValue(serviceName, out var list))
        {
            list = new List<ServiceAddress>();
        }

        return Task.FromResult(list);
    }

    private void Register(string serviceName, ServiceAddress address)
    {
        _logger.LogInformation($"Register service: [{serviceName}@{address}]");

        if (!_serviceRegistry.TryGetValue(serviceName, out var list))
        {
            list = new List<ServiceAddress>();
        }

        list.Add(address);
        _serviceRegistry[serviceName] = list;
    }
}
