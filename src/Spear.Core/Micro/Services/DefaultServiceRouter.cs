using System.Reflection;
using Microsoft.Extensions.Logging;
using Spear.Core.Config;
using Spear.Core.Extensions;

namespace Spear.Core.Micro.Services
{
    public class DefaultServiceRouter : DServiceFinder, IServiceRegister
    {
        private readonly Dictionary<string, List<ServiceAddress>> _serviceCenter;
        public DefaultServiceRouter(ILogger<DefaultServiceRouter> logger)
            : base(null, logger)
        {
            _serviceCenter = new Dictionary<string, List<ServiceAddress>>();
            RegistFromConfig();
        }

        private void RegistFromConfig()
        {
            var services = SpearConfig.GetConfig().Services;
            if (services.IsNullOrEmpty())
                return;

            foreach (var service in services)
            {
                if (service.Key.IsNullOrEmpty() || service.Value.IsNullOrEmpty())
                    continue;

                foreach (var address in service.Value)
                {
                    Register(service.Key, address);
                }
            }
        }

        public Task Deregister()
        {
            _serviceCenter.Clear();
            return Task.CompletedTask;
        }

        public void Register(string serviceName, ServiceAddress address)
        {
            Logger?.LogInformation($"regist service:{serviceName},{address}");
            if (!_serviceCenter.TryGetValue(serviceName, out var list))
            {
                list = new List<ServiceAddress>();
            }
            list.Add(address);
            _serviceCenter[serviceName] = list;
        }

        public Task Register(IEnumerable<Assembly> assemblyList, ServiceAddress serverAddress)
        {
            foreach (var assembly in assemblyList)
            {
                var serviveName = assembly.ServiceName();
                Register(serviveName, serverAddress);
            }
            return Task.CompletedTask;
        }

        protected override Task<List<ServiceAddress>> QueryService(Type serviceType, ProductMode[] modes)
        {
            var serviceName = serviceType.Assembly.ServiceName();
            if (!_serviceCenter.TryGetValue(serviceName, out var list))
            {
                list = new List<ServiceAddress>();
            }
            return Task.FromResult(list);

        }
    }
}
