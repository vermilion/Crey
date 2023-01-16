namespace Crey.Messages;

/// <summary>
/// Invoke context
/// </summary>
public class MessageInvokeContext
{
    public MessageInvokeContext()
    {
    }

    public MessageInvokeContext(MessageInvokeContext? context)
    {
        var ctx = context ?? new MessageInvokeContext();

        Type = ctx.Type;
        Headers = ctx.Headers.ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Invoke type
    /// </summary>
    public MessageInvokeContextType Type { get; set; } = MessageInvokeContextType.None;

    /// <summary>
    /// Headers passed
    /// </summary>
    public IDictionary<string, string?> Headers { get; set; } = new Dictionary<string, string?>();
}
