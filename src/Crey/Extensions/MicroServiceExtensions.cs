using Crey.Micro;
using Crey.Session;

namespace Crey.Extensions;

public static class MicroServiceExtensions
{
    public static async Task InvokeWithContextValues<T>(this T service, Func<T, Task> action, IDictionary<string, object?> values)
        where T : class, IMicroService
    {
        CallContextProvider.Current.Clear();

        try
        {
            foreach (var item in values)
            {
                CallContextProvider.Current.Add(item.Key, item.Value?.ToString());
            }

            await action(service);
        }
        catch
        {
            CallContextProvider.Current.Clear();
            throw;
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
        return service.InvokeWithContextValues(action, new Dictionary<string, object?>
        {
            [MicroConstants.LongRunning] = true
        });
    }
}
