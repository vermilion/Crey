using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace Crey.Helper;

/// <summary>
/// Utilities class for working with IP
/// </summary>
internal static class IpAddressHelper
{
    private static IPAddress? _localIp;

    public static IPAddress? LocalIp()
    {
        _localIp ??= ResolveIPAddressOrDefault(null, null, AddressFamily.InterNetwork);

        return _localIp;
    }

    public static IPAddress? ResolveIPAddressOrDefault(string? addrOrHost, byte[]? subnet, AddressFamily family)
    {
        // if the address is an empty string, just enumerate all ip addresses available
        if (string.IsNullOrEmpty(addrOrHost))
            return ResolveIPAddressUsingNetworkInterfaces(subnet, family);

        // check if addrOrHost is a valid IP address including loopback (127.0.0.0/8, ::1) and any (0.0.0.0/0, ::) addresses
        if (IPAddress.TryParse(addrOrHost, out var address))
            return address;

        // Get IP address from DNS. If addrOrHost is localhost will 
        // return loopback IPv4 address (or IPv4 and IPv6 addresses if OS is supported IPv6)
        var nodeIps = Dns.GetHostAddresses(addrOrHost);
        return PickIPAddress(nodeIps, subnet, family);
    }

    private static IPAddress? ResolveIPAddressUsingNetworkInterfaces(byte[]? subnet, AddressFamily family)
    {
        var nodeIps = NetworkInterface.GetAllNetworkInterfaces()
            .Where(iface => iface.OperationalStatus == OperationalStatus.Up)
            .SelectMany(iface => iface.GetIPProperties().UnicastAddresses)
            .Select(addr => addr.Address)
            .Where(addr => addr.AddressFamily == family && !IPAddress.IsLoopback(addr))
            .ToList();

        var ipAddress = PickIPAddress(nodeIps, subnet, family);
        return ipAddress;
    }

    private static IPAddress? PickIPAddress(IList<IPAddress> nodeIps, byte[]? subnet, AddressFamily family)
    {
        var candidates = new List<IPAddress>();
        foreach (var nodeIp in nodeIps.Where(x => x.AddressFamily == family))
        {
            // If the subnet does not match - we can't resolve this address.
            // If subnet is not specified - pick smallest address deterministically.
            if (subnet == null)
            {
                candidates.Add(nodeIp);
            }
            else
            {
                var ip = nodeIp;
                if (subnet.Select((b, i) => ip.GetAddressBytes()[i] == b).All(x => x))
                {
                    candidates.Add(nodeIp);
                }
            }
        }

        return candidates.Count > 0 ? PickIPAddress(candidates) : null;
    }

    private static IPAddress? PickIPAddress(IReadOnlyList<IPAddress> candidates)
    {
        IPAddress? chosen = null;
        foreach (var addr in candidates)
        {
            if (chosen == null)
            {
                chosen = addr;
            }
            else
            {
                if (CompareIPAddresses(addr, chosen)) // pick smallest address deterministically
                    chosen = addr;
            }
        }

        return chosen;

        // returns true if lhs is "less" (in some repeatable sense) than rhs
        static bool CompareIPAddresses(IPAddress lhs, IPAddress rhs)
        {
            var lbytes = lhs.GetAddressBytes();
            var rbytes = rhs.GetAddressBytes();

            if (lbytes.Length != rbytes.Length)
                return lbytes.Length < rbytes.Length;

            // compare starting from most significant octet.
            // 10.68.20.21 < 10.98.05.04
            for (var i = 0; i < lbytes.Length; i++)
            {
                if (lbytes[i] != rbytes[i])
                {
                    return lbytes[i] < rbytes[i];
                }
            }

            // They're equal
            return false;
        }
    }
}
