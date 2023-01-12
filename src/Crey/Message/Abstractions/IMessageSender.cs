namespace Crey.Message;

public interface IMessageSender
{
    Task Send(Message message, bool flush = true);
}
