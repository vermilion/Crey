using Microsoft.Extensions.Logging;

namespace Spear.Core.Micro.Services
{
    public abstract class DServiceFinder : DServiceRoute, IServiceFinder
    {
        protected readonly ILogger Logger;

        protected DServiceFinder(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary> 查询服务 </summary>
        /// <param name="serviceType"></param>
        /// <param name="modes"></param>
        /// <returns></returns>
        protected abstract Task<List<ServiceAddress>> QueryService(Type serviceType, ProductMode[] modes);

        /// <summary> 服务发现 </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public async Task<List<ServiceAddress>> Find(Type serviceType)
        {
            var modes = new List<ProductMode> { Constants.Mode };
            if (Constants.Mode == ProductMode.Dev)
                modes.Add(ProductMode.Test);

            return await QueryService(serviceType, modes.ToArray());
        }
    }
}
