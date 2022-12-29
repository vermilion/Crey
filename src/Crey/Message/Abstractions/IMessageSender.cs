using Crey.Message.Models;

namespace Crey.Message.Abstractions;

public interface IMessageSender
{
    Task Send(DMessage message, bool flush = true);
}
