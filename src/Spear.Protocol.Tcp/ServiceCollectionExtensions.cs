using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Builder;
using Spear.Core.Micro.Abstractions;

namespace Spear.Protocol.Tcp
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> 使用DotNetty的TCP传输协议 </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMicroBuilder AddTcpProtocol(this IMicroBuilder builder)
        {
            builder.Services.AddSingleton<IMicroListener, DotNettyMicroListener>();
            builder.Services.AddSingleton<IMicroClientFactory, DotNettyClientFactory>();

            return builder;
        }
    }
}
