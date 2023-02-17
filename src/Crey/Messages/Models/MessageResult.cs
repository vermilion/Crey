namespace Crey.Messages;

/// <summary>
/// Result message object
/// </summary>
public class MessageResult : Message
{
    /// <summary>
    /// Indicates result Status
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Text message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Message content
    /// </summary>
    public object Content { get; set; }
}
