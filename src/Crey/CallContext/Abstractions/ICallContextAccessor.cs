namespace Crey.CallContext;

public interface ICallContextAccessor
{
    MessageInvokeContext Context { get; internal set; }
}
