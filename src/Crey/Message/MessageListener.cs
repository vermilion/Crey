namespace Crey.Message;

public class MessageListener : IMessageListener
{
    public event ReceivedDelegate Received;

    public async Task OnReceived(IMessageSender sender, DMessage message)
    {
        if (Received == null)
            return;

        await Received(sender, message);
    }
}
