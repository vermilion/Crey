using Spear.Core.Extensions;
using Spear.Micro.Abstractions;

namespace Spear.Micro.Extensions;

public static class MicroSessionExtensions
{
    public static T GetValue<T>(this IMicroSession session, string key, T def = default)
    {
        if (session.Values.TryGetValue(key, out var result))
            return result.CastTo<T>();

        return def;
    }
}
