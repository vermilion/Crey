using System.Security.Claims;
using Spear.Core.Session.Abstractions;
using Spear.Core.Session.Models;

namespace Spear.Core.Session.Extensions;

public static class PrincipalAccessorExtensions
{
    public static void SetSession(this IPrincipalAccessor accessor, SessionDto session)
    {
        if (accessor?.Principal == null) return;
        var claims = new List<Claim>();
        if (session.UserId != null)
        {
            claims.AddRange(new[]
            {
                new Claim(SpearClaimTypes.UserId, session.UserId?.ToString()),
                new Claim(SpearClaimTypes.UserName, session.UserName ?? string.Empty),
                new Claim(SpearClaimTypes.Role, session.Role ?? string.Empty)
            });
        }
        if (!claims.Any()) return;
        var identity = new ClaimsIdentity(claims);
        accessor.Principal.AddIdentity(identity);
    }

    public static string GetValue(this IPrincipalAccessor accessor, string type)
    {
        var claim = accessor.Principal?.Claims.FirstOrDefault(t => t.Type == type);
        return string.IsNullOrWhiteSpace(claim?.Value) ? null : claim.Value;
    }

    public static void SetValue(this IPrincipalAccessor accessor, string type, string value)
    {
        if (accessor.Principal.HasClaim(type, value))
            return;

        accessor.Principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(type, value) }));
    }
}
