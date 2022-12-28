namespace Psi.Micro.Abstractions;

public interface IMicroSession
{
    Dictionary<string, string> Values { get; }

    void Set<T>(string key, T value);
}
