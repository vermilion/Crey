using Psi.Extensions;
using Psi.Micro.Abstractions;

namespace Psi.Micro.Extensions;

public static class MicroSessionExtensions
{
    public static T GetValue<T>(this IMicroSession session, string key, T def = default)
    {
        if (session.Values.TryGetValue(key, out var result))
            return result.CastTo<T>();

        return def;
    }
}
