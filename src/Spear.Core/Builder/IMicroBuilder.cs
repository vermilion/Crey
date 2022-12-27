using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Spear.Core.Builder
{
    /// <summary> Spear 服务构建器 </summary>
    public interface IBuilder
    {
        IConfiguration Configuration { get; }

        /// <summary> 服务集合 </summary>
        IServiceCollection Services { get; }
    }

    /// <summary> Spear客户端构建器 </summary>
    public interface IMicroClientBuilder : IBuilder { }

    /// <summary> Spear服务端构建器 </summary>
    public interface IMicroServerBuilder : IBuilder { }

    public interface IMicroBuilder : IMicroClientBuilder, IMicroServerBuilder
    {
    }
}
