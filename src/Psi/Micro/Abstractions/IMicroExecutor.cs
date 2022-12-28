using Psi.Message.Abstractions;
using Psi.Message.Models;

namespace Psi.Micro.Abstractions;

public interface IMicroExecutor
{
    Task Execute(IMessageSender sender, InvokeMessage message);
}
