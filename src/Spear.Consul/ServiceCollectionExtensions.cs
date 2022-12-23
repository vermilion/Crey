using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Core.Micro;
using Spear.Core.Micro.Services;
using Spear.Discovery.Consul;

namespace Spear.Consul
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 使用Consul作为服务注册和发现的组件
        /// 读取配置：consul
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionAction"></param>
        /// <returns></returns>
        public static IMicroClientBuilder AddConsul(this IMicroClientBuilder builder, Action<ConsulOption> optionAction = null)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IServiceFinder>(provider =>
            {
                var option = ConsulOption.Config();
                optionAction?.Invoke(option);
                var cache = provider.GetService<IMemoryCache>();
                var logger = provider.GetService<ILogger<ConsulServiceFinder>>();
                return new ConsulServiceFinder(cache, logger, option.Server, option.Token);
            });
            return builder;
        }

        /// <summary>
        /// 使用Consul作为服务注册和发现的组件
        /// 读取配置：consul
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionAction"></param>
        /// <returns></returns>
        public static IMicroServerBuilder AddConsul(this IMicroServerBuilder builder, Action<ConsulOption> optionAction = null)
        {
            builder.Services.AddSingleton<IServiceRegister>(provider =>
            {
                var option = ConsulOption.Config();
                optionAction?.Invoke(option);
                var logger = provider.GetService<ILogger<ConsulServiceRegister>>();
                return new ConsulServiceRegister(logger, option.Server, option.Token);
            });
            return builder;
        }

        /// <summary> 使用Consul作为服务注册和发现的组件 </summary>
        /// <param name="builder"></param>
        /// <param name="server"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IMicroClientBuilder AddConsul(this IMicroClientBuilder builder, string server, string token = null)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IServiceFinder>(provider =>
            {
                var cache = provider.GetService<IMemoryCache>();
                var logger = provider.GetService<ILogger<ConsulServiceFinder>>();
                return new ConsulServiceFinder(cache, logger, server, token);
            });
            return builder;
        }

        /// <summary> 使用Consul作为服务注册和发现的组件 </summary>
        /// <param name="builder"></param>
        /// <param name="server"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IMicroServerBuilder AddConsul(this IMicroServerBuilder builder, string server, string token = null)
        {
            builder.Services.AddSingleton<IServiceRegister>(provider =>
            {
                var logger = provider.GetService<ILogger<ConsulServiceRegister>>();
                return new ConsulServiceRegister(logger, server, token);
            });
            return builder;
        }
    }
}
