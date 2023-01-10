using Crey.Discovery;

namespace Crey.Micro;

public interface IMicroClientFactory
{
    Task<IMicroClient> CreateClient(ServiceAddress serviceAddress);
}
