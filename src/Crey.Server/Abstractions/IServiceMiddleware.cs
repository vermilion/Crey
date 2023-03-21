namespace Crey.Service;

/// <summary>
/// Represents an async continuation for the next task to execute in the pipeline
/// </summary>
public delegate Task NextServiceDelegate();

/// <summary>
/// Middleware interface
/// </summary>
public interface IServiceMiddleware
{
    /// <summary>
    /// Handler to execute
    /// </summary>
    /// <param name="message">Received message</param>
    /// <param name="next">Continuation delegate</param>
    /// <returns><see cref="Task"/></returns>
    Task Execute(MessageInvoke message, NextServiceDelegate next);
}
