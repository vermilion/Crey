namespace Castle.DynamicProxy;

/// <summary>
/// Extension methods to <see cref="IProxyGenerator"/>.
/// </summary>
internal static class ProxyGeneratorExtensions
{
    /// <summary>
    /// Creates an <see cref="IInterceptor"/> for the supplied <paramref name="interceptor"/>.
    /// </summary>
    /// <param name="interceptor">The interceptor for asynchronous operations.</param>
    /// <returns>The <see cref="IInterceptor"/> for the supplied <paramref name="interceptor"/>.</returns>
    public static IInterceptor ToInterceptor(this IAsyncInterceptor interceptor)
    {
        return new AsyncDeterminationInterceptor(interceptor);
    }
}
