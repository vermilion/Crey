using Psi.Message.Models;

namespace Psi.Message.Abstractions;

public interface IMessageSender
{
    Task Send(DMessage message, bool flush = true);
}
