using Crey.Session.Abstractions;

namespace Crey.Session;

internal class SessionValuesAccessor : ISessionValuesAccessor
{
    public Dictionary<string, string?> Values { get; } = new();
}
