using Crey.Micro.Abstractions;

namespace Crey.Micro;

public class MicroSession : IMicroSession
{
    public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

    public void Set<T>(string key, T value)
    {
        Values.Add(key, value?.ToString() ?? "");
    }
}
