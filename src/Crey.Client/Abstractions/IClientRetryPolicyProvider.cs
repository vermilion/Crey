using Polly;

namespace Crey.ClientSide;

public interface IClientRetryPolicyProvider
{
    IAsyncPolicy Provide(Action<Exception, TimeSpan, int, Context> onRetry);
}
