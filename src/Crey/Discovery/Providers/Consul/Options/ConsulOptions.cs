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
