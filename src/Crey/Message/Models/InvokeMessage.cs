namespace Crey.Message;

public class InvokeMessage : Message
{
    /// <summary>
    /// Service unique identifier
    /// </summary>
    public string ServiceId { get; set; }

    /// <summary>
    /// List of parameters used to invoke a service
    /// </summary>
    public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Current Invoke context
    /// </summary>
    public InvokeMethodContext Context { get; set; } = new();
}

/// <summary>
/// Invoke context
/// </summary>
public class InvokeMethodContext
{
    public InvokeMethodContext()
    {
    }

    public InvokeMethodContext(InvokeMethodContext? context)
    {
        var ctx = context ?? new InvokeMethodContext();

        Type = ctx.Type;
        Headers = ctx.Headers.ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Invoke type
    /// </summary>
    public InvokeMethodType Type { get; set; } = InvokeMethodType.None;

    /// <summary>
    /// Headers passed
    /// </summary>
    public IDictionary<string, string?> Headers { get; set; } = new Dictionary<string, string?>();
}
