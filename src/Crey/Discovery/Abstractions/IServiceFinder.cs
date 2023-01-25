namespace Crey.Discovery;

/// <summary>
/// Service discovery finder
/// </summary>
public interface IServiceFinder
{
    /// <summary>
    /// Starts service monitoring routine
    /// </summary>
    /// <param name="cancellationToken">Token <see cref="CancellationToken"/></param>
    /// <returns><see cref="Task"/></returns>
    Task MonitorAliveServices(CancellationToken cancellationToken = default);

    /// <summary>
    /// Allows to get list of alive services for given contract type
    /// </summary>
    /// <param name="serviceType">Contract type</param>
    /// <returns>Address list</returns>
    Task<List<ServiceAddress>> QueryService(Type serviceType);
}
