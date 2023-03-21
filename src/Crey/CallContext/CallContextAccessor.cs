namespace Crey.CallContext;

public class CallContextAccessor : ICallContextAccessor
{
    public MessageInvokeContext Context { get; set; } = new();

    public MessageInvokeContext GetContext()
    {
       return Context;
    }
}
