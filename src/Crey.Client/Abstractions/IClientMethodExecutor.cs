using System.Reflection;
using Crey.Messages;

namespace Crey.ClientSide;

public interface IClientMethodExecutor
{
    Task<MessageResult> Execute(MethodInfo targetMethod, IDictionary<string, object> args);
}
