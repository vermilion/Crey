using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Crey.Service;

internal class PostConfigureServiceAddress : IPostConfigureOptions<ServiceAddress>
{
    private readonly ILogger<PostConfigureServiceAddress> _logger;

    public PostConfigureServiceAddress(ILogger<PostConfigureServiceAddress> logger)
    {
        _logger = logger;
    }

    public void PostConfigure(string? name, ServiceAddress options)
    {
        try
        {
            var resolvedIP = IpAddressHelper.ResolveIPAddressOrDefault(options.Host, _logger);

            if (resolvedIP is null)
                throw new Exception($"Unable to find a suitable candidate for {nameof(ServiceAddress)}. {nameof(options.Host)}");

            options.Host = resolvedIP.ToString();
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex.Message, ex);

            throw;
        }
    }
}