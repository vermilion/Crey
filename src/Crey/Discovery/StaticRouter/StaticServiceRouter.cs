using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Crey.Extensions;
using Crey.Extensions.StringExtension;

namespace Crey.Discovery.StaticRouter;

public class StaticServiceRouter : ServiceFinder, IServiceRegister
{
    private readonly Dictionary<string, List<ServiceAddress>> _serviceCenter = new();

    public StaticServiceRouter(
        IOptions<StaticRouterOptions> options,
        ILogger<StaticServiceRouter> logger)
        : base(logger)
    {
        RegisterFromConfig(options.Value);
    }

    private void RegisterFromConfig(StaticRouterOptions options)
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
        _serviceCenter.Clear();
        return Task.CompletedTask;
    }

    public void Register(string serviceName, ServiceAddress address)
    {
        Logger?.LogInformation($"Register service: [{serviceName}@{address}]");

        if (!_serviceCenter.TryGetValue(serviceName, out var list))
        {
            list = new List<ServiceAddress>();
        }

        list.Add(address);
        _serviceCenter[serviceName] = list;
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

    public override Task<List<ServiceAddress>> QueryService(Type serviceType)
    {
        var serviceName = serviceType.Assembly.ServiceName();
        if (!_serviceCenter.TryGetValue(serviceName, out var list))
        {
            list = new List<ServiceAddress>();
        }

        return Task.FromResult(list);
    }
}
