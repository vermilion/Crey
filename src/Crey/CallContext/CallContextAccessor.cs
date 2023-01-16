namespace Crey.CallContext;

internal class CallContextAccessor : ICallContextAccessor
{
    public MessageInvokeContext Context { get; set; } = new();

    public MessageInvokeContext GetContext()
    {
       return Context;
    }
}
