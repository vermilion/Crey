using Spear.Core.Message.Models;

namespace Spear.Core.Micro.Abstractions
{
    /// <summary> 微服务客户端 </summary>
    public interface IMicroClient
    {
        /// <summary> 发送消息 </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<MessageResult> Send(InvokeMessage message);
    }
}
