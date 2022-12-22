using System.Security.Claims;

namespace Spear.Core.Session
{
    public static class SpearClaimTypes
    {
        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.Name"/>
        /// </summary>
        public static string UserName { get; set; } = ClaimTypes.Name;

        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.NameIdentifier"/>
        /// </summary>
        public static string UserId { get; set; } = ClaimTypes.NameIdentifier;

        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.Role"/>
        /// </summary>
        public static string Role { get; set; } = ClaimTypes.Role;

        public const string TraceId = "TraceId";

        public const string HeaderAuthorization = "Authorization";
        public const string HeaderUserAgent = "User-Agent";
        public const string HeaderReferer = "referer";
        public const string HeaderForward = "X-Forwarded-For";
        public const string HeaderRealIp = "X-Real-IP";
        public const string HeaderTraceId = "Trace-Id";

        public static string HeaderUserId { get; set; } = "User-Id";
        public static string HeaderUserName { get; set; } = "User-Name";
        public static string HeaderRole { get; set; } = "User-Role";
    }
}
