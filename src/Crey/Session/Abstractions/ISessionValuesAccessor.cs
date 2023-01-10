namespace Crey.Session;

public interface ISessionValuesAccessor
{
    public Dictionary<string, string?> Values { get; }
}
