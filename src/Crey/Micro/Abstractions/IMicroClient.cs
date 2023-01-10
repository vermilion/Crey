using Crey.Message;

namespace Crey.Micro;

public interface IMicroClient
{
    Task<MessageResult> Send(InvokeMessage message);
}
