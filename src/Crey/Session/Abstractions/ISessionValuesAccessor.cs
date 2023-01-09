namespace Crey.Session.Abstractions;

public interface ISessionValuesAccessor
{
    public Dictionary<string, string?> Values { get; }
}
