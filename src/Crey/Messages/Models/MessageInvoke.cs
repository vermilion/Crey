namespace Crey.Messages;

public class MessageInvoke : Message
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
    public MessageInvokeContext Context { get; set; } = new();
}
