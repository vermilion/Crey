namespace Crey.ClientSide;

/// <summary>
/// Represents an async continuation for the next task to execute in the pipeline
/// </summary>
public delegate Task<MessageResult> ClientHandlerDelegate();

/// <summary>
/// Middleware interface
/// </summary>
public interface IClientMiddleware
{
    /// <summary>
    /// Handler to execute
    /// </summary>
    /// <param name="message">Received message</param>
    /// <param name="next">Continuation delegate</param>
    /// <returns><see cref="Task"/></returns>
    Task<MessageResult> Execute(MessageInvoke message, ClientHandlerDelegate next);
}
