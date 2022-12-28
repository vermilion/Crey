using Psi.Message.Models;

namespace Psi.Message.Abstractions;

public delegate Task ReceivedDelegate(IMessageSender sender, DMessage message);

public interface IMessageListener
{
    event ReceivedDelegate Received;

    Task OnReceived(IMessageSender sender, DMessage message);
}
