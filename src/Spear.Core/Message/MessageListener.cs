using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;

namespace Spear.Core.Message;

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
