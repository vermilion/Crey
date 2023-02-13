using System.Net.Sockets;
using Polly;

namespace Crey.ClientSide;

internal class ClientRetryPolicyProvider : IClientRetryPolicyProvider
{
    public IAsyncPolicy Provide(Action<Exception, TimeSpan, int, Context> onRetry)
    {
        var builder = Policy
            .Handle<Exception>(ex => ex.GetBaseException() is SocketException);

        // retry 5 times
        return builder.WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(500), onRetry);
    }
}
