using Spear.Core.Message.Abstractions;
using Spear.Core.Message.Models;

namespace Spear.Core.Micro.Abstractions;

public interface IMicroExecutor
{
    Task Execute(IMessageSender sender, InvokeMessage message);
}
