using Crey.Message;

namespace Crey.Session;

internal class CallContextAccessor : ICallContextAccessor
{
    public InvokeMethodContext Context { get; set; } = new();

    public InvokeMethodContext GetContext()
    {
       return Context;
    }
}
