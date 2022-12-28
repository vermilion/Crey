using Spear.Core.Message.Models;

namespace Spear.Core.Micro.Abstractions;

public interface IMicroClient
{
    Task<MessageResult> Send(InvokeMessage message);
}
