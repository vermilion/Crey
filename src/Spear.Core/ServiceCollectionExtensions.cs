using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Config;
using Spear.Core.Extensions;
using Spear.Core.Micro;
using Spear.Core.Micro.Implementation;
using Spear.Core.Micro.Services;
using Spear.Core.Proxy;
using Spear.Core.Session;
using Spear.Core.Session.Impl;
using Spear.ProxyGenerator;

namespace Spear.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> 使用Session。 </summary>
        /// <param name="builder">服务构建者。</param>
        /// <returns>服务构建者。</returns>
        public static IMicroClientBuilder AddSession(this IMicroClientBuilder builder)
        {
            //Session
            builder.AddSession<DefaultPrincipalAccessor>();
            return builder;
        }

        /// <summary> 使用Session。 </summary>
        /// <param name="builder">服务构建者。</param>
        /// <returns>服务构建者。</returns>
        public static IMicroClientBuilder AddSession<T>(this IMicroClientBuilder builder)
            where T : class, IPrincipalAccessor
        {
            //Session
            builder.Services.AddScoped<IPrincipalAccessor, T>();
            builder.Services.AddScoped<IMicroSession, ClaimMicroSession>();
            return builder;
        }

        /// <summary> 使用Session。 </summary>
        /// <param name="builder">服务构建者。</param>
        /// <returns>服务构建者。</returns>
        public static IMicroServerBuilder AddSession(this IMicroServerBuilder builder)
        {
            //Session
            builder.AddSession<DefaultPrincipalAccessor>();
            return builder;
        }

        /// <summary> 使用Session。 </summary>
        /// <param name="builder">服务构建者。</param>
        /// <returns>服务构建者。</returns>
        public static IMicroServerBuilder AddSession<T>(this IMicroServerBuilder builder)
            where T : class, IPrincipalAccessor
        {
            //Session
            builder.Services.AddScoped<IPrincipalAccessor, T>();
            builder.Services.AddScoped<IMicroSession, ClaimMicroSession>();
            return builder;
        }

        /// <summary> 添加默认服务路由 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="routerAction"></param>
        /// <returns></returns>
        public static IMicroServerBuilder AddDefaultRouter(this IMicroServerBuilder builder, Action<DefaultServiceRouter> routerAction = null)
        {
            builder.Services.AddSingleton<DefaultServiceRouter>();
            builder.Services.AddSingleton<IServiceRegister>(provider =>
            {
                var router = provider.GetService<DefaultServiceRouter>();
                routerAction?.Invoke(router);
                return router;
            });
            return builder;
        }

        /// <summary> 添加默认服务路由 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="routerAction"></param>
        /// <returns></returns>
        public static IMicroClientBuilder AddDefaultRouter(this IMicroClientBuilder builder, Action<DefaultServiceRouter> routerAction = null)
        {
            builder.Services.AddSingleton<DefaultServiceRouter>();
            builder.Services.AddSingleton<IServiceFinder>(provider =>
            {
                var router = provider.GetService<DefaultServiceRouter>();
                routerAction?.Invoke(router);
                return router;
            });
            return builder;
        }

        /// <summary> 添加微服务客户端 </summary>
        /// <param name="services"></param>
        /// <param name="builderAction"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static MicroBuilder AddMicroClient(this MicroBuilder services, Action<IMicroClientBuilder> builderAction, Action<SpearConfig> configAction = null)
        {
            //services.TryAddSingleton<Counter>();
            var config = SpearConfig.GetConfig();
            configAction?.Invoke(config);
            services.Services.AddSingleton(config);
            services.Services.AddProxy<ClientProxy>();
            builderAction.Invoke(services);
            return services;
        }

        /// <summary> 添加微服务 </summary>
        /// <param name="services"></param>
        /// <param name="builderAction"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static MicroBuilder AddMicroService(this MicroBuilder services,
            Action<IMicroServerBuilder> builderAction, Action<SpearConfig> configAction = null)
        {
            var config = "spear".Config<SpearConfig>() ?? new SpearConfig();
            configAction?.Invoke(config);
            services.Services.AddSingleton(config);

            services.Services.AddSingleton(services.Services);
            services.Services.AddSingleton<IMicroEntryFactory, MicroEntryFactory>();
            builderAction.Invoke(services);
            services.Services.AddSingleton<IMicroExecutor, MicroExecutor>();
            services.Services.AddSingleton<IMicroHost, MicroHost>();
            //services.TryAddSingleton<Counter>();
            return services;
        }


        /// <summary> 开启微服务 </summary>
        /// <param name="provider"></param>
        /// <param name="addressAction"></param>
        /// <returns></returns>
        public static async Task UseMicroService(this IServiceProvider provider, Action<ServiceAddress> addressAction = null)
        {
            var address = SpearConfig.GetConfig().Service;
            addressAction?.Invoke(address);
            var host = provider.GetService<IMicroHost>();

            await host.Start(address);
        }
    }
}
