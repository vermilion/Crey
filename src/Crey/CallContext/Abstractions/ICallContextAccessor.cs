namespace Crey.CallContext;

public interface ICallContextAccessor
{
    MessageInvokeContext Context { get; set; }
}
