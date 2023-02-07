using System.Reflection;

namespace Crey.Clients;

public interface IClientMethodExecutor
{
    Task<MessageResult> Execute(MethodInfo targetMethod, IDictionary<string, object> args);
}
