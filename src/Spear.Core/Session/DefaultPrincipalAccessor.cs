using System.Security.Claims;
using Spear.Core.Session.Abstractions;

namespace Spear.Core.Session;

public class DefaultPrincipalAccessor : IPrincipalAccessor
{
    public ClaimsPrincipal Principal
    {
        get
        {
            if (Thread.CurrentPrincipal is ClaimsPrincipal principal)
                return principal;

            principal = new ClaimsPrincipal();
            Thread.CurrentPrincipal = principal;
            return principal;
        }
    }
}
