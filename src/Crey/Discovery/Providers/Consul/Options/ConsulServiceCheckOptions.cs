namespace Crey.Discovery.Consul;

public class ConsulServiceCheckOptions
{
    public int DeregisterCriticalServiceAfterDays { get; set; } = 0;
    public int Timeout { get; set; } = 5;
    public int Interval { get; set; } = 1;
}
