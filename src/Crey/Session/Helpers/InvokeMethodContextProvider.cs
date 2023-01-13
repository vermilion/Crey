using Crey.Message;

namespace Crey.Session;

public class InvokeMethodContextProvider
{
    private static readonly AsyncLocal<InvokeMethodContext?> _context = new();

    public static InvokeMethodContext? Context
    {
        get => _context.Value;
        set => _context.Value = value;
    }
}
