﻿namespace Crey.Discovery;

/// <summary>
/// Service discovery finder
/// </summary>
public interface IServiceFinder
{
    /// <summary>
    /// Allows to get list of alive services for given contract type
    /// </summary>
    /// <param name="serviceType">Contract type</param>
    /// <returns>Address list</returns>
    Task<List<ServiceAddress>> QueryServices(Type serviceType);
}
