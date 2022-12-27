using System.Security.Claims;

namespace Spear.Core.Session.Models;

public static class SpearClaimTypes
{
    /// <summary>
    /// User Name
    /// Default: <see cref="ClaimTypes.Name"/>
    /// </summary>
    public static string UserName { get; set; } = ClaimTypes.Name;

    /// <summary>
    /// UserId
    /// Default: <see cref="ClaimTypes.NameIdentifier"/>
    /// </summary>
    public static string UserId { get; set; } = ClaimTypes.NameIdentifier;

    /// <summary>
    /// Role
    /// Default: <see cref="ClaimTypes.Role"/>
    /// </summary>
    public static string Role { get; set; } = ClaimTypes.Role;

    public const string TraceId = "TraceId";

    public const string HeaderAuthorization = "Authorization";
    public const string HeaderUserAgent = "User-Agent";
    public const string HeaderReferer = "referer";
    public const string HeaderUserIp = "X-Real-IP";
    public const string HeaderTraceId = "Trace-Id";

    public static string HeaderUserId { get; set; } = "User-Id";
    public static string HeaderUserName { get; set; } = "User-Name";
    public static string HeaderRole { get; set; } = "User-Role";
}
