namespace Crey.Messages;

/// <summary>
/// Invocation type
/// </summary>
public enum MessageInvokeContextType
{
    /// <summary>
    /// No options defined
    /// </summary>
    None = 0,

    /// <summary>
    /// Invocation is one-way with no feedback on whether the call succeeds or fails
    /// </summary>
    OneWay = 1
}