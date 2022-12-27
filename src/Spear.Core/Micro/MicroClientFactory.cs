using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery;

namespace Spear.Core.Micro
{
    public abstract class MicroClientFactory : IMicroClientFactory, IDisposable
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;
        protected readonly IServiceProvider Provider;
        protected readonly IMicroExecutor MicroExecutor;

        private readonly ConcurrentDictionary<ServiceAddress, Lazy<Task<IMicroClient>>> _clients;

        protected MicroClientFactory(ILoggerFactory loggerFactory, IServiceProvider provider, IMicroExecutor microExecutor)
        {
            Provider = provider;
            MicroExecutor = microExecutor;
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(GetType());
            _clients = new ConcurrentDictionary<ServiceAddress, Lazy<Task<IMicroClient>>>();
        }

        /// <summary> 创建客户端 </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected abstract Task<IMicroClient> Create(ServiceAddress address);

        /// <summary> 删除客户端缓存 </summary>
        /// <param name="address"></param>
        protected void Remove(ServiceAddress address)
        {
            Logger.LogInformation($"移除客户端:{address}");
            _clients.TryRemove(address, out _);
        }

        public async Task<IMicroClient> CreateClient(ServiceAddress serviceAddress)
        {
            try
            {
                var lazyClient = _clients.GetOrAdd(serviceAddress,
                    key => new Lazy<Task<IMicroClient>>(async () =>
                    {
                        Logger.LogInformation($"创建客户端：{key}创建客户端。");
                        return await Create(key);
                    }));

                return await lazyClient.Value;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"创建客户端失败:{serviceAddress}");
                Remove(serviceAddress);
                throw;
            }
        }

        public void Dispose()
        {
            foreach (var client in _clients.Values.Where(i => i.IsValueCreated))
            {
                (client.Value as IDisposable)?.Dispose();
            }
        }
    }
}
