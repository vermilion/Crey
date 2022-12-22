using System.Linq;

namespace Spear.Core.Session.Impl
{
    public class ClaimMicroSession : AbstractMicroSession
    {
        private readonly IPrincipalAccessor _principalAccessor;


        public ClaimMicroSession(IPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }

        private string GetClaimValue(string type)
        {
            var claim = _principalAccessor.Principal?.Claims.FirstOrDefault(t => t.Type == type);
            return string.IsNullOrWhiteSpace(claim?.Value) ? null : claim.Value;
        }

        public override string UserName =>
            TempSession != null ? TempSession.UserName : GetClaimValue(SpearClaimTypes.UserName);

        public override string Role => TempSession != null ? TempSession.Role : GetClaimValue(SpearClaimTypes.Role);

        protected override object GetUserId()
        {
            return GetClaimValue(SpearClaimTypes.UserId);
        }
    }
}
