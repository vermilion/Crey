using Crey.Message;

namespace Crey.Session;

public interface ICallContextAccessor
{
    InvokeMethodContext Context { get; internal set; }
}
