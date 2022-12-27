using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spear.Core.Builder;
using Spear.Core.ServiceDiscovery.Abstractions;
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
        public static IMicroBuilder AddConsul(this IMicroBuilder builder, Action<ConsulOption> optionAction = null)
        {
            builder.Services.Configure<ConsulOption>(builder.Configuration.GetSection("consul"));

            builder.Services.AddSingleton<IServiceRegister>(provider =>
            {
                var option = provider.GetRequiredService<IOptions<ConsulOption>>().Value;
                optionAction?.Invoke(option);
                var logger = provider.GetRequiredService<ILogger<ConsulServiceRegister>>();
                return new ConsulServiceRegister(logger, option.Server, option.Token);
            });

            builder.Services.AddSingleton<IServiceFinder>(provider =>
            {
                var option = provider.GetRequiredService<IOptions<ConsulOption>>().Value;
                optionAction?.Invoke(option);
                var logger = provider.GetRequiredService<ILogger<ConsulServiceFinder>>();
                return new ConsulServiceFinder(logger, option.Server, option.Token);
            });

            return builder;
        }

        /// <summary> 使用Consul作为服务注册和发现的组件 </summary>
        /// <param name="builder"></param>
        /// <param name="server"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IMicroBuilder AddConsul(this IMicroBuilder builder, string server, string token = null)
        {
            builder.Services.AddSingleton<IServiceRegister>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ConsulServiceRegister>>();
                return new ConsulServiceRegister(logger, server, token);
            });

            builder.Services.AddSingleton<IServiceFinder>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ConsulServiceFinder>>();
                return new ConsulServiceFinder(logger, server, token);
            });

            return builder;
        }
    }
}
