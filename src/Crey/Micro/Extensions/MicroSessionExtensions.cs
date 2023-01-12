using Crey.Extensions;
using Crey.Session;

namespace Crey.Micro;

public static class MicroSessionExtensions
{
    public static T? GetValue<T>(this ICallContextAccessor accessor, string key, T? def = default)
    {
        if (accessor.Context.Headers.TryGetValue(key, out var result))
            return result.CastTo<T?>();

        return def;
    }
}
