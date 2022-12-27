using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Spear.Core.Micro.Abstractions;
using Spear.Core.ServiceDiscovery;
using Spear.Core.ServiceDiscovery.Abstractions;

namespace Spear.Core.Micro
{
    /// <summary> 服务宿主 </summary>
    public class MicroHost : MicroHostBase
    {
        private readonly IServiceRegister _serviceRegister;
        private readonly IMicroEntryFactory _entryFactory;
        private readonly ILogger<MicroHost> _logger;

        /// <summary> 服务宿主机 </summary>
        /// <param name="serviceExecutor"></param>
        /// <param name="microListener"></param>
        /// <param name="serviceRegister"></param>
        /// <param name="entryFactory"></param>
        /// <param name="loggerFactory"></param>
        public MicroHost(IMicroExecutor serviceExecutor, IMicroListener microListener,
            IServiceRegister serviceRegister, IMicroEntryFactory entryFactory,
            ILoggerFactory loggerFactory)
            : base(serviceExecutor, microListener, loggerFactory)
        {
            _serviceRegister = serviceRegister;
            _entryFactory = entryFactory;
            _logger = loggerFactory.CreateLogger<MicroHost>();
        }

        public override void Dispose()
        {
            (MicroListener as IDisposable)?.Dispose();
        }

        /// <inheritdoc />
        /// <summary> 启动服务 </summary>
        /// <param name="serviceAddress">主机终结点。</param>
        /// <returns>一个任务。</returns>
        public override async Task Start(ServiceAddress serviceAddress)
        {
            try
            {
                await MicroListener.Start(serviceAddress);

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation($"Service started at：{serviceAddress}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            var assemblies = _entryFactory.GetContracts();
            await _serviceRegister.Register(assemblies, serviceAddress);
        }

        /// <summary> 停止服务 </summary>
        /// <returns></returns>
        public override async Task Stop()
        {
            await _serviceRegister.Deregister();
            await MicroListener.Stop();
            Console.WriteLine("Service Stoped");
        }
    }
}
