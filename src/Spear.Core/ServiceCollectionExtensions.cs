using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Background;
using Spear.Core.Builder;
using Spear.Core.Micro;
using Spear.Core.Micro.Abstractions;
using Spear.Core.Options;
using Spear.Core.Proxy;
using Spear.Core.ServiceDiscovery;
using Spear.Core.ServiceDiscovery.Abstractions;
using Spear.Core.Session;
using Spear.Core.Session.Abstractions;
using Spear.Core.StaticRouter;
using Spear.ProxyGenerator;

namespace Spear.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> 使用Session。 </summary>
        /// <param name="builder">服务构建者。</param>
        /// <returns>服务构建者。</returns>
        public static IMicroBuilder AddSession(this IMicroBuilder builder)
        {
            //Session
            builder.AddSession<DefaultPrincipalAccessor>();
            return builder;
        }

        /// <summary> 使用Session。 </summary>
        /// <param name="builder">服务构建者。</param>
        /// <returns>服务构建者。</returns>
        public static IMicroBuilder AddSession<T>(this IMicroBuilder builder)
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
        public static IMicroBuilder AddDefaultRouter(this IMicroBuilder builder, Action<DefaultRouterOptions> routerAction = null)
        {
            var config = builder.Configuration.GetSection("micro:router").Get<DefaultRouterOptions>();
            routerAction?.Invoke(config);

            //builder.Services.Configure<DefaultRouterOptions>();

            builder.Services.AddSingleton<IServiceRegister, StaticServiceRouter>();
            builder.Services.AddSingleton<IServiceFinder, StaticServiceRouter>();
            return builder;
        }

        /// <summary> 添加微服务客户端 </summary>
        /// <param name="builder"></param>
        /// <param name="builderAction"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IMicroBuilder AddMicroClient(this IMicroBuilder builder, Action<IMicroClientBuilder> builderAction)
        {
            builder.Services.AddProxy<ClientProxy>();
            builderAction.Invoke(builder);

            return builder;
        }

        /// <summary> 添加微服务 </summary>
        /// <param name="builder"></param>
        /// <param name="builderAction"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IMicroBuilder AddMicroService(this IMicroBuilder builder, Action<IMicroServerBuilder> builderAction, Action<DefaultRouterOptions> configAction = null)
        {
            var services = builder.Services;

            services.AddSingleton(services);
            services.AddSingleton<IMicroEntryFactory, MicroEntryFactory>();
            services.AddSingleton<IMicroExecutor, MicroExecutor>();
            services.AddSingleton<IMicroHost, MicroHost>();

            services.AddHostedService<HostListenerBackroungService>();

            builderAction.Invoke(builder);

            services.Configure<ServiceAddress>(builder.Configuration.GetSection("micro"));

            return builder;
        }
    }
}
