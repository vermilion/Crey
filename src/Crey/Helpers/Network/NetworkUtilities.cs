using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Crey.Helpers;
public class NetworkUtilities
{
    private readonly NetworkOptions _options;
    private readonly ILogger _logger;

    public NetworkUtilities(NetworkOptions options, ILogger logger = null)
    {
        _options = options;
        _logger = logger;
    }

    public IPAddress? FindFirstNonLoopbackAddress()
    {
        IPAddress? result = null;

        try
        {
            var lowest = int.MaxValue;
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var @interface in interfaces)
            {
                if (@interface.OperationalStatus == OperationalStatus.Up && !@interface.IsReceiveOnly)
                {
                    _logger?.LogDebug("Testing interface: {name}, {id}", @interface.Name, @interface.Id);

                    var props = @interface.GetIPProperties();
                    var ipProps = props.GetIPv4Properties();

                    if (ipProps.Index < lowest || result == null)
                    {
                        lowest = ipProps.Index;
                    }
                    else if (result != null)
                    {
                        continue;
                    }

                    if (!IgnoreInterface(@interface.Name))
                    {
                        foreach (UnicastIPAddressInformation addressInfo in props.UnicastAddresses)
                        {
                            IPAddress address = addressInfo.Address;

                            if (IsInet4Address(address) && !IsLoopbackAddress(address) && IsPreferredAddress(address))
                            {
                                _logger?.LogTrace("Found non-loopback interface: {name}", @interface.Name);
                                result = address;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Cannot get first non-loopback address");
        }

        return result;
    }

    private bool IsInet4Address(IPAddress address)
    {
        return address.AddressFamily == AddressFamily.InterNetwork;
    }

    private bool IsLoopbackAddress(IPAddress address)
    {
        return IPAddress.IsLoopback(address);
    }

    private bool IsPreferredAddress(IPAddress address)
    {
        if (_options.UseOnlySiteLocalInterfaces)
        {
            var siteLocalAddress = IsSiteLocalAddress(address);

            if (!siteLocalAddress)
            {
                _logger?.LogDebug("Ignoring address: {address} [UseOnlySiteLocalInterfaces=true, this address is not]", address);
            }

            return siteLocalAddress;
        }

        IEnumerable<string> preferredNetworks = _options.GetPreferredNetworks();

        if (!preferredNetworks.Any())
        {
            return true;
        }

        foreach (var regex in preferredNetworks)
        {
            string hostAddress = address.ToString();
            var matcher = new Regex(regex);

            if (matcher.IsMatch(hostAddress) || hostAddress.StartsWith(regex, StringComparison.Ordinal))
            {
                return true;
            }
        }

        _logger?.LogDebug("Ignoring address: {address}", address);
        return false;
    }

    internal bool IgnoreInterface(string interfaceName)
    {
        if (string.IsNullOrEmpty(interfaceName))
        {
            return false;
        }

        foreach (string regex in _options.GetIgnoredInterfaces())
        {
            var matcher = new Regex(regex);

            if (matcher.IsMatch(interfaceName))
            {
                _logger?.LogTrace("Ignoring interface: {name}", interfaceName);
                return true;
            }
        }

        return false;
    }

    internal bool IsSiteLocalAddress(IPAddress address)
    {
        string text = address.ToString();

        return text.StartsWith("10.", StringComparison.Ordinal) ||
            text.StartsWith("172.16.", StringComparison.Ordinal) ||
            text.StartsWith("192.168.", StringComparison.Ordinal);
    }
}