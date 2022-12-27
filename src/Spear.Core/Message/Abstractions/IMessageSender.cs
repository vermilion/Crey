using Spear.Core.Message.Models;

namespace Spear.Core.Message.Abstractions;

public interface IMessageSender
{
    Task Send(DMessage message, bool flush = true);
}
