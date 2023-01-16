namespace Crey.Messages;

public delegate Task ReceivedDelegate(IMessageSender sender, Message message);

public interface IMessageListener
{
    event ReceivedDelegate Received;

    Task OnReceived(IMessageSender sender, Message message);
}
