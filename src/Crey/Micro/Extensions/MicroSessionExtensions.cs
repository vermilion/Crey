using Crey.Extensions;
using Crey.Session.Abstractions;

namespace Crey.Micro.Extensions;

public static class MicroSessionExtensions
{
    public static T? GetValue<T>(this ISessionValuesAccessor session, string key, T? def = default)
    {
        if (session.Values.TryGetValue(key, out var result))
            return result.CastTo<T?>();

        return def;
    }
}
