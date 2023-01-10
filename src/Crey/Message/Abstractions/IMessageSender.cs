namespace Crey.Message;

public interface IMessageSender
{
    Task Send(DMessage message, bool flush = true);
}
