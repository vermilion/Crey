using Crey.Message.Abstractions;
using Crey.Message.Models;

namespace Crey.Micro.Abstractions;

public interface IMicroExecutor
{
    Task Execute(IMessageSender sender, InvokeMessage message);
}
