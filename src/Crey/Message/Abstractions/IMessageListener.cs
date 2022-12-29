using Crey.Message.Models;

namespace Crey.Message.Abstractions;

public delegate Task ReceivedDelegate(IMessageSender sender, DMessage message);

public interface IMessageListener
{
    event ReceivedDelegate Received;

    Task OnReceived(IMessageSender sender, DMessage message);
}
