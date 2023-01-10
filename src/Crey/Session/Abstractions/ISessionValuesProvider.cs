namespace Crey.Session;

public interface ISessionValuesProvider : ISessionValuesAccessor
{
    void SetValue(string key, string? value);
}
