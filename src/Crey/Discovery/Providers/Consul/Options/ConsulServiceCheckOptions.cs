namespace Crey.Discovery.Consul;

public class ConsulServiceCheckOptions
{
    public TimeSpan DeregisterCriticalServiceAfter { get; set; } = TimeSpan.FromHours(1);
    public int Timeout { get; set; } = 5;
    public int Interval { get; set; } = 1;
}
