using Crey.Message.Models;

namespace Crey.Micro.Abstractions;

public interface IMicroClient
{
    Task<MessageResult> Send(InvokeMessage message);
}
