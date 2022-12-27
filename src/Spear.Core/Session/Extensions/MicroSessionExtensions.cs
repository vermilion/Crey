using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Core.Session.Abstractions;

namespace Spear.Core.Session.Extensions;

public static class MicroSessionExtensions
{
    public static T GetUserId<T>(this IMicroSession session, T def = default)
    {
        return session.UserId.CastTo(def);
    }

    public static T GetRequiredUserId<T>(this IMicroSession session)
    {
        var value = session.UserId.CastTo<T>();
        if (Equals(value, default(T)))
            throw new SpearException("userId not found");

        return value;
    }
}
