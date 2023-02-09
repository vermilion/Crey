namespace Crey.ClientSide;

public interface IClientFactory
{
    Task<IClient> CreateClient(ServiceAddress serviceAddress);
}
