using System.Net;
using Microsoft.Extensions.Logging;

namespace Crey.Helpers;

/// <summary>
/// Utilities class for working with IP
/// </summary>
internal static class IpAddressHelper
{
    private static IPAddress? _localIp;

    public static IPAddress? LocalIp()
    {
        _localIp ??= ResolveIPAddressOrDefault(null, null);

        return _localIp;
    }

    public static IPAddress? ResolveIPAddressOrDefault(string? addrOrHost, ILogger? logger)
    {
        var utils = new NetworkUtilities(new NetworkOptions
        {
            PreferredNetworks = addrOrHost
        }, logger);

        return utils.FindFirstNonLoopbackAddress();
    }
}