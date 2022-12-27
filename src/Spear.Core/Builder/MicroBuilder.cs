using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Spear.Core.Builder
{
    /// <summary> Spear构建器 </summary>
    public class MicroBuilder : IMicroBuilder
    {
        public MicroBuilder(IConfiguration configuration, IServiceCollection services)
        {
            Configuration = configuration;
            Services = services;
        }

        public IConfiguration Configuration { get; }

        /// <summary> 服务集合 </summary>
        public IServiceCollection Services { get; }
    }
}
