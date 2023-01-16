namespace Crey.CallContext;

public class InvokeMethodContextProvider
{
    private static readonly AsyncLocal<MessageInvokeContext?> _context = new();

    public static MessageInvokeContext? Context
    {
        get => _context.Value;
        set => _context.Value = value;
    }
}
