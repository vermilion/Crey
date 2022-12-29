using System.Text.RegularExpressions;

namespace Psi.Extensions.StringExtension;

public static class RegexExtension
{
    private const string IpRegex = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

    public static bool IsIp(this string ip)
    {
        return !string.IsNullOrWhiteSpace(ip) && Regex.IsMatch(ip, IpRegex);
    }
}
