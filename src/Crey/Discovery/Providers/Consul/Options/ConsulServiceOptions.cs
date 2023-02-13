namespace Crey.Discovery.Consul;

public class ConsulServiceOptions
{
    /// <summary>
    /// Additional tags
    /// </summary>
    public IEnumerable<string> Tags { get; set; }

    /// <summary>
    /// Additional metadata key-value properties
    /// </summary>
    public Dictionary<string, string> Meta { get; set; }

    /// <summary>
    /// Service check options
    /// </summary>
    public ConsulServiceCheckOptions Check { get; set; } = new();
}
