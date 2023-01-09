namespace Crey.Session.Abstractions;

public interface ISessionValuesProvider : ISessionValuesAccessor
{
    void SetValue(string key, string? value);
}
