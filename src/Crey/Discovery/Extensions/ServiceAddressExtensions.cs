using System.Net;
using System.Text.RegularExpressions;

namespace Crey.Discovery;

public static class ServiceAddressExtensions
{
    public static EndPoint ToEndpoint(this ServiceAddress address)
    {
        var ipRegex = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

        if (Regex.IsMatch(address.Host, ipRegex))
            return new IPEndPoint(IPAddress.Parse(address.Host), address.Port);

        return new DnsEndPoint(address.Host, address.Port);
    }
}
