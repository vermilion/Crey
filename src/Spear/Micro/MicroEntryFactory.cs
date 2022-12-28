using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Core.Extensions;
using Spear.Core.Micro.Abstractions;
using Spear.ProxyGenerator.Helpers;

namespace Spear.Core.Micro;

public class MicroEntryFactory : IMicroEntryFactory
{
    private readonly ILogger<MicroEntryFactory> _logger;
    private readonly ConcurrentDictionary<string, MicroEntryDelegate> _entries = new();
    private readonly List<Type> _services = new();

    public MicroEntryFactory(ILogger<MicroEntryFactory> logger, IServiceProvider serviceProvider)
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
                if (FilterMethod(method))
                    continue;

                var serviceId = GenerateServiceId(method);
                _entries.TryAdd(serviceId, CreateEntry(method));
            }
        }
    }

    private MicroEntryDelegate CreateEntry(MethodInfo method)
    {
        var fastInvoke = FastInvokeHelper.GetMethodInvoker(method);

        return (provider, param) =>
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

            return Task.FromResult(fastInvoke(instance, args.ToArray()));
        };
    }

    protected virtual List<Type> FindServices(IServiceProvider serviceProvider)
    {
        var collection = serviceProvider.GetRequiredService<IServiceCollection>();

        return collection
             .Where(x => typeof(IMicroService).IsAssignableFrom(x.ServiceType))
             .Select(x => x.ServiceType)
             .ToList();
    }

    protected virtual bool FilterMethod(MethodInfo method)
    {
        return false;
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

    public MicroEntryDelegate? Find(string serviceId)
    {
        if (_entries.TryGetValue(serviceId, out var method))
            return method;

        return null;
    }
}
