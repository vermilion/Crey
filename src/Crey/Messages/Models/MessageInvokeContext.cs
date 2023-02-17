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

        Headers = ctx.Headers.ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Correlation Identifier
    /// </summary>
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Headers passed
    /// </summary>
    public IDictionary<string, string?> Headers { get; set; } = new Dictionary<string, string?>();
}
