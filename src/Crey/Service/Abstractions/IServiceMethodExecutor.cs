namespace Crey.Service;

public interface IServiceMethodExecutor
{
    Task Execute(IMessageSender sender, MessageInvoke message);
}