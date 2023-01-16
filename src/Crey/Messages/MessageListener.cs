namespace Crey.Messages;

public class MessageListener : IMessageListener
{
    public event ReceivedDelegate Received;

    public async Task OnReceived(IMessageSender sender, Message message)
    {
        if (Received == null)
            return;

        await Received(sender, message);
    }
}
