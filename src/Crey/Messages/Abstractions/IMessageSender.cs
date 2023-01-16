namespace Crey.Messages;

public interface IMessageSender
{
    Task Send(Message message, bool flush = true);
}
