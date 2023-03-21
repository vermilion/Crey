namespace Crey.Service;

/// <summary>
/// Service address info
/// </summary>
public class ServiceAddress
{
    /// <summary>
    /// Service Host
    /// </summary>
    public string Host { get; set; }

    private int _port;

    /// <summary>
    /// Service Port
    /// </summary>
    public int Port
    {
        get => _port > 80 ? _port : 5000;
        set => _port = value;
    }

    /// <summary>
    /// Service weight - can be used for weighted service selection
    /// </summary>
    public double Weight { get; set; } = 1;

    public ServiceAddress() { }

    public ServiceAddress(string host, int port)
    {
        Host = host;
        Port = port;
    }

    public override string ToString()
    {
        return $"{Host}:{Port}";
    }
}
