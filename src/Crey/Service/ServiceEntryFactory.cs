using System.Collections.Concurrent;
using System.Reflection;
using Crey.Contracts;
using Crey.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crey.Service;

public class ServiceEntryFactory : IServiceEntryFactory
{
    private readonly ILogger<ServiceEntryFactory> _logger;
    private readonly ConcurrentDictionary<string, ServiceEntryInfo> _entries = new();
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
            _services.Add(service.ServiceType);
            var methods = service.ServiceType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var serviceId = GenerateServiceId(method);
                _entries.TryAdd(serviceId, CreateEntry(service.ImplementationType, method));
            }
        }
    }

    private ServiceEntryInfo CreateEntry(Type? implementationType, MethodInfo method)
    {
        var fastInvokeHandler = FastInvokeHelper.GetMethodInvoker(method);

        Task<object> DelegateMethod(IServiceProvider provider, IDictionary<string, object> param)
        {
            var instance = provider.GetService(method.DeclaringType);

            var args = new List<object>();
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

        // create entry
        var entry = new ServiceEntryInfo(DelegateMethod);

        var targetMethod = GetImplementedMethod(implementationType, method);
        var attributes = targetMethod?.GetCustomAttributes<MethodFilterAttribute>(true);
        if (attributes is not null)
            entry.MethodFilters = attributes.Select(x => x.Type).ToList();

        return entry;
    }

    protected virtual List<(Type ServiceType, Type? ImplementationType)> FindServices(IServiceProvider serviceProvider)
    {
        var collection = serviceProvider.GetRequiredService<IServiceCollection>();

        return collection
             .Where(x => typeof(IMicroService).IsAssignableFrom(x.ServiceType))
             .Select(x => (x.ServiceType, x.ImplementationType))
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

    public ServiceEntryInfo? Find(string serviceId)
    {
        if (_entries.TryGetValue(serviceId, out var method))
            return method;

        return null;
    }

    private MethodInfo? GetImplementedMethod(Type? targetType, MethodInfo? interfaceMethod)
    {
        if (targetType is null) throw new ArgumentNullException(nameof(targetType));
        if (interfaceMethod is null) throw new ArgumentNullException(nameof(interfaceMethod));

        var map = targetType.GetInterfaceMap(interfaceMethod.DeclaringType);
        var index = Array.IndexOf(map.InterfaceMethods, interfaceMethod);
        if (index < 0)
            return null;

        return map.TargetMethods[index];

    }
}
