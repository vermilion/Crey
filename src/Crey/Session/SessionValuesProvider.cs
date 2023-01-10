namespace Crey.Session;

internal class SessionValuesProvider : ISessionValuesProvider
{
    public Dictionary<string, string?> Values => CallContextProvider.Current;

    public void SetValue(string key, string? value)
    {
        CallContextProvider.Current.Add(key, value);
    }
}
