using Crey.Message;

namespace Crey.Micro;

public interface IMicroExecutor
{
    Task Execute(IMessageSender sender, InvokeMessage message);
}
