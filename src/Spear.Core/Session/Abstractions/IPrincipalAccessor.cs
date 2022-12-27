using System.Security.Claims;

namespace Spear.Core.Session.Abstractions;

public interface IPrincipalAccessor
{
    ClaimsPrincipal Principal { get; }
}
