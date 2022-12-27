using System.Collections.Concurrent;
using Spear.ProxyGenerator.Abstractions;

namespace Spear.ProxyGenerator;

internal class ProxyResolver : IResolver
{
    private readonly ConcurrentDictionary<Type, IDictionary<object, object>> _initializers = new();

    private static object ConvertKey(object key)
    {
        return key ?? "{{NULL}}";
    }

    public virtual void Register(Type type, object value, object key = null)
    {
        key = ConvertKey(key);
        if (_initializers.ContainsKey(type))
        {
            if (_initializers.TryGetValue(type, out var dict))
                dict[key] = value;
        }
        else
        {
            _initializers.TryAdd(type, new Dictionary<object, object> { { key, value } });
        }
    }

    public virtual object Resolve(Type type, object key = null)
    {
        key = ConvertKey(key);
        if (_initializers.TryGetValue(type, out var result) && result.TryGetValue(key, out var service))
        {
            return service;
        }

        return null;
    }
}
