namespace Crey.Discovery;

/// <summary>
/// Options used to configure discovery
/// </summary>
public class DiscoveryOptions
{
    /// <summary>
    /// Interval used to fetch alive services in background
    /// </summary>
    public int FetchInterval { get; set; } = 60;
}