using Spear.Micro.Abstractions;

namespace Spear.Micro;

public class MicroSession : IMicroSession
{
    public Guid Id = Guid.NewGuid();

    public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

    public void Set<T>(string key, T value)
    {
        Values.Add(key, value?.ToString() ?? "");
    }
}
