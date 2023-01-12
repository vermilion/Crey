using Crey.Message;
using Crey.Micro;
using Crey.Session;

namespace Crey.Extensions;

public static class MicroServiceExtensions
{
    public static async Task InvokeWithContextValues<T>(this T service, Func<T, Task> action, InvokeMethodContext context)
        where T : class, IMicroService
    {
        try
        {
            InvokeMethodContextProvider.Set(context);

            await action(service);
        }
        finally
        {
            InvokeMethodContextProvider.Reset();
        }
    }

    /// <summary>
    /// Invoke service "OneWay", without waiting for result
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <param name="service">Service instance</param>
    /// <param name="action">Action to execute</param>
    /// <returns><see cref="Task"/> that completes once service receives request</returns>
    public static Task InvokeOneWay<T>(this T service, Func<T, Task> action)
        where T : class, IMicroService
    {
        return service.InvokeWithContextValues(action, new InvokeMethodContext
        {
            Type = InvokeMethodType.OneWay
        });
    }
}
