namespace Crey.Discovery.Consul;

/// <summary>
/// Options used to configure Consul provider
/// </summary>
public class ConsulOptions
{
    /// <summary>
    /// Server URL
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    /// Token used to connect to server
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Service related options
    /// </summary>
    public ConsulServiceOptions Service { get; set; } = new();
}

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
public class ConsulServiceCheckOptions
{
    public int DeregisterCriticalServiceAfterDays { get; set; } = 0;
    public int Timeout { get; set; } = 5;
    public int Interval { get; set; } = 1;
}
