using Crey.Extensions;
using Crey.Session;

namespace Crey.Micro;

public static class MicroSessionExtensions
{
    public static T? GetValue<T>(this ISessionValuesAccessor session, string key, T? def = default)
    {
        if (session.Values.TryGetValue(key, out var result))
            return result.CastTo<T?>();

        return def;
    }
}
