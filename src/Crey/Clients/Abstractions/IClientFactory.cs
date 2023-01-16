namespace Crey.Clients;

public interface IClientFactory
{
    Task<IClient> CreateClient(ServiceAddress serviceAddress);
}
