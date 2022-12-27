using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Core.Extensions;
using Spear.Core.Micro.Abstractions;
using Spear.Core.Micro.Models;
using Spear.ProxyGenerator.Helpers;

namespace Spear.Core.Micro;

public class MicroEntryFactory : IMicroEntryFactory
{
    private readonly ILogger<MicroEntryFactory> _logger;
    private readonly ConcurrentDictionary<string, MicroEntry> _entries;
    private readonly IServiceProvider _provider;

    public MicroEntryFactory(ILogger<MicroEntryFactory> logger, IServiceProvider provider)
    {
        _entries = new ConcurrentDictionary<string, MicroEntry>();
        Services = new List<Type>();
        _logger = logger;
        _provider = provider;

        InitServices();
    }

    private void InitServices()
    {
        var services = FindServices();
        foreach (var service in services)
        {
            Services.Add(service);
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

    private MicroEntry CreateEntry(MethodInfo method)
    {
        var fastInvoke = FastInvokeHelper.GetMethodInvoker(method);

        return new MicroEntry(method)
        {
            Invoke = param =>
            {
                var instance = _provider.GetService(method.DeclaringType);
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
            }
        };
    }

    protected virtual List<Type> FindServices()
    {
        var collection = _provider.GetService<IServiceCollection>();

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

    public List<Type> Services { get; }

    public virtual IEnumerable<Assembly> GetContracts()
    {
        var list = new List<Assembly>();
        foreach (var service in Services)
        {
            var ass = service.Assembly;
            if (list.Contains(ass))
                continue;

            list.Add(ass);
        }

        return list;
    }

    public MicroEntry? Find(string serviceId)
    {
        if (_entries.TryGetValue(serviceId, out var method))
            return method;

        return null;
    }
}
