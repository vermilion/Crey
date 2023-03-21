namespace Castle.DynamicProxy;

/// <summary>
/// Implement this interface to intercept method invocations with DynamicProxy2.
/// </summary>
public interface IAsyncInterceptor
{
    /// <summary>
    /// Intercepts a synchronous method <paramref name="invocation"/>.
    /// </summary>
    /// <param name="invocation">The method invocation.</param>
    void InterceptSynchronous(IInvocation invocation);

    /// <summary>
    /// Intercepts an asynchronous method <paramref name="invocation"/> with return type of <see cref="Task"/>.
    /// </summary>
    /// <param name="invocation">The method invocation.</param>
    void InterceptAsynchronous(IInvocation invocation);

    /// <summary>
    /// Intercepts an asynchronous method <paramref name="invocation"/> with return type of <see cref="Task{T}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the <see cref="Task{T}"/> <see cref="Task{T}.Result"/>.</typeparam>
    /// <param name="invocation">The method invocation.</param>
    void InterceptAsynchronous<TResult>(IInvocation invocation);
}
