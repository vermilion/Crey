using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace Psi.Helper;

public static class IpAddressHelper
{
    private static string _localIp;

    public static string LocalIp()
    {
        if (!string.IsNullOrWhiteSpace(_localIp))
            return _localIp;

        return _localIp = NetworkInterface
            .GetAllNetworkInterfaces()
            .Select(p => p.GetIPProperties())
            .SelectMany(p => p.UnicastAddresses)
            .FirstOrDefault(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))?.Address?.ToString();
    }
}
