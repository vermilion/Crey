namespace Psi.ServiceDiscovery.Models;

public class ServiceAddress
{
    public string Host { get; set; }

    private int _port;

    public int Port
    {
        get => _port > 80 ? _port : 5000;
        set => _port = value;
    }

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
