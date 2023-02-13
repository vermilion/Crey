namespace Crey.ClientSide;

/// <summary>
/// Defines client creation factory
/// </summary>
public interface IClientFactory
{
    Task<IClient> CreateClient(ServiceAddress serviceAddress);
}
