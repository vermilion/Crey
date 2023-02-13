namespace Crey.Exceptions;

/// <summary>
/// Provides methods for exception convertion to transport object and vise versa
/// </summary>
public interface IExceptionConverter
{
    /// <summary>
    /// Wraps exception to transport object
    /// </summary>
    /// <param name="ex">Exception object</param>
    /// <returns>Transport object representing exception</returns>
    MessageException Wrap(Exception ex);

    /// <summary>
    /// Unwraps exception from message object
    /// </summary>
    /// <param name="message">Transport object representing exception type</param>
    /// <returns>Exception <see cref="FaultException"/></returns>
    FaultException Unwrap(MessageException? message);
}
