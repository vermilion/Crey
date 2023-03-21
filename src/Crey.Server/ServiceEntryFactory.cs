using System.Collections.Concurrent;
using System.Reflection;
using Crey.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Crey.Service;

public class ServiceEntryFactory : IServiceEntryFactory
{
    private readonly ILogger<ServiceEntryFactory> _logger;
    private readonly ConcurrentDictionary<string, ServiceEntryDelegate> _entries = new();
    private readonly List<Type> _services = new();

    public ServiceEntryFactory(ILogger<ServiceEntryFactory> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;

        InitServices(serviceProvider);
    }

    private void InitServices(IServiceProvider serviceProvider)
    {
        var services = FindServices(serviceProvider);

        foreach (var service in services)
        {
            _services.Add(service);
            var methods = service.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var serviceId = GenerateServiceId(method);
                _entries.TryAdd(serviceId, CreateEntry(method));
            }
        }
    }

    private ServiceEntryDelegate CreateEntry(MethodInfo method)
    {
        var fastInvokeHandler = FastInvokeHelper.GetMethodInvoker(method);

        Task<object> DelegateMethod(IServiceProvider provider, IDictionary<string, object> param)
        {
            var instance = provider.GetService(method.DeclaringType);

            var args = new List<object?>();
            var parameters = param ?? new Dictionary<string, object>();
            foreach (var parameter in method.GetParameters())
            {
                if (parameters.ContainsKey(parameter.Name))
                {
                    var parameterType = parameter.ParameterType;
                    var arg = parameters[parameter.Name].CastTo(parameterType);
                    args.Add(arg);
                }
                else if (parameter.HasDefaultValue)
                {
                    args.Add(parameter.DefaultValue);
                }
            }

            return Task.FromResult(fastInvokeHandler(instance, args.ToArray()));
        }

        return DelegateMethod;
    }

    protected virtual List<Type> FindServices(IServiceProvider serviceProvider)
    {
        var collection = serviceProvider.GetRequiredService<IServiceCollection>();

        return collection
             .Where(x => typeof(IMicroService).IsAssignableFrom(x.ServiceType))
             .Select(x => x.ServiceType)
             .ToList();
    }

    protected virtual string GenerateServiceId(MethodInfo method)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));

        var type = method.DeclaringType;
        if (type == null)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Declaration type can't be null");

            throw new ArgumentNullException(nameof(method.DeclaringType), "Declaration type can't be null");
        }

        var id = method.ServiceKey();

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug($"Method：{method} - generated Id：{id}");

        return id;
    }

    public virtual IEnumerable<Assembly> GetContracts()
    {
        var list = new List<Assembly>();
        foreach (var service in _services)
        {
            var ass = service.Assembly;
            if (list.Contains(ass))
                continue;

            list.Add(ass);
        }

        return list;
    }

    public ServiceEntryDelegate? Find(string serviceId)
    {
        if (_entries.TryGetValue(serviceId, out var method))
            return method;

        return null;
    }
}
