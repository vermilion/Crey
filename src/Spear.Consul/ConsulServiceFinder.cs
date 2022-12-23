using System.Text.Json;
using Consul;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Spear.Core;
using Spear.Core.Extensions;
using Spear.Core.Micro.Services;

namespace Spear.Consul
{
    public class ConsulServiceFinder : DServiceFinder
    {
        private readonly string _consulServer;
        private readonly string _consulToken;

        public ConsulServiceFinder(IMemoryCache cache, ILogger logger, string server, string token = null)
            : base(cache, logger)
        {
            _consulServer = server;
            _consulToken = token;
        }

        private IConsulClient CreateClient()
        {
            return new ConsulClient(cfg =>
            {
                cfg.Address = new Uri(_consulServer);
                if (!string.IsNullOrWhiteSpace(_consulToken))
                    cfg.Token = _consulToken;
            });
        }

        protected override async Task<List<ServiceAddress>> QueryService(Type serviceType, ProductMode[] modes)
        {
            var services = new List<ServiceAddress>();

            using var client = CreateClient();
            var list = await client.Catalog.Service(serviceType.Assembly.ServiceName());
            foreach (var service in list.Response)
            {
                if (service.ServiceMeta.TryGetValue(KeyMode, out var modeValue))
                {
                    var mode = modeValue.CastTo(ProductMode.Dev);
                    if (!modes.Contains(mode))
                        continue;
                }

                if (service.ServiceMeta.TryGetValue(KeyService, out var json))
                {
                    var address = JsonSerializer.Deserialize<ServiceAddress>(json);
                    if (address is not null)
                        services.Add(address);
                }
            }

            return services;
        }
    }
}
