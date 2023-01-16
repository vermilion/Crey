namespace Crey.Clients;

public interface IClient
{
    Task<MessageResult> Send(MessageInvoke message);
}
