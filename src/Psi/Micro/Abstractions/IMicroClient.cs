using Psi.Message.Models;

namespace Psi.Micro.Abstractions;

public interface IMicroClient
{
    Task<MessageResult> Send(InvokeMessage message);
}
