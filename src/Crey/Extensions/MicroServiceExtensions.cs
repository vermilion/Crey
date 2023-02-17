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
}
