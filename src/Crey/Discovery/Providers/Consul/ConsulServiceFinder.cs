using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Crey.Extensions;
using Crey.Helper;
using Microsoft.Extensions.Caching.Memory;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Crey.Discovery.Consul;

internal class ConsulServiceFinder : IServiceFinder
{
    private readonly ILogger<ConsulServiceRegister> _logger;
    private readonly ConsulOptions _options;
    private readonly SemaphoreSlim _slimlock = new(1, 1);
    private readonly IMemoryCache _memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions
    {
        CompactionPercentage = 0.05,
        ExpirationScanFrequency = new TimeSpan(0, 0, 1),
    }));

    private const uint CacheSeconds = 5;

    public ConsulServiceFinder(
        ILogger<ConsulServiceRegister> logger,
        IOptions<ConsulOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    private IConsulClient CreateClient()
    {
        return new ConsulClient(cfg =>
        {
            cfg.Address = new Uri(_options.Server);
            if (!string.IsNullOrWhiteSpace(_options.Token))
                cfg.Token = _options.Token;
        });
    }

    public async Task<List<ServiceAddress>> QueryService(Type serviceType)
    {
        if (TryGetFromCache(serviceType.Name, out var cachedServices))
            return cachedServices;

        await _slimlock.WaitAsync();

        try
        {
            if (TryGetFromCache(serviceType.Name, out var cachedServicesLocked))
                return cachedServicesLocked;

            var services = await GetAllAliveServices(serviceType);

            if (services.Any())
            {
                _memoryCache.Set(serviceType.Name, services, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheSeconds)
                });
            }

            return services;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
        finally
        {
            _slimlock.Release();
        }
    }

    private async Task<List<ServiceAddress>> GetAllAliveServices(Type serviceType)
    {
        var services = new List<ServiceAddress>();

        using var client = CreateClient();
        var list = await client.Health.Service(serviceType.Assembly.ServiceName(), null, true);

        foreach (var entry in list.Response)
        {
            var service = entry.Service;

            if (service.Meta.TryGetValue(ConsulRouteConstants.KeyService, out var json))
            {
                var address = JsonHelper.FromJson<ServiceAddress>(json);
                if (address is not null)
                    services.Add(address);
            }
        }

        return services;
    }

    private bool TryGetFromCache(string serviceName, out List<ServiceAddress>? services)
    {
        services = _memoryCache.Get<List<ServiceAddress>>(serviceName);

        if (services is not null)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Cache hit for service: {serviceName}");

            return true;
        }

        return false;
    }
}
