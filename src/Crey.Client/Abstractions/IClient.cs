using Crey.Messages;

namespace Crey.ClientSide;

public interface IClient
{
    Task<MessageResult> Send(MessageInvoke message);
}
