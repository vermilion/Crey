using Crey.Extensions;

namespace Crey.CallContext;

public static class CallContextAccessorExtensions
{
    public static T? GetValue<T>(this ICallContextAccessor accessor, string key, T? def = default)
    {
        if (accessor.Context.Headers.TryGetValue(key, out var result))
            return result.CastTo<T?>();

        return def;
    }
}
