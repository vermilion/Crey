using Crey.Contracts;

namespace Crey.Extensions;

public static class MicroServiceExtensions
{
    public static async Task InvokeWithContextValues<T>(this T service, Func<T, Task> action, MessageInvokeContext context)
        where T : class, IMicroService
    {
        InvokeMethodContextProvider.Context = context;

        try
        {
            await action(service);
        }
        finally
        {
            InvokeMethodContextProvider.Context = null;
        }
    }

    /// <summary>
    /// Invoke service "OneWay", in fire-and-forget manner
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <param name="service">Service instance</param>
    /// <param name="action">Action to execute</param>
    /// <returns><see cref="Task"/> that completes once service receives request</returns>
    public static Task InvokeOneWay<T>(this T service, Func<T, Task> action)
        where T : class, IMicroService
    {
        return service.InvokeWithContextValues(action, new MessageInvokeContext
        {
            Type = MessageInvokeContextType.OneWay
        });
    }
}
