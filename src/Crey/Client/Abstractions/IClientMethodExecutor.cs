using System.Reflection;

namespace Crey.Client;

public interface IClientMethodExecutor
{
    Task<MessageResult> Execute(MethodInfo targetMethod, IDictionary<string, object> args);
}
