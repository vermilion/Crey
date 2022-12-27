using System.Threading.Tasks;
using Spear.Core.Message.Models;
using Spear.Core.Message.Abstractions;

namespace Spear.Core.Micro.Abstractions
{
    /// <summary> 服务执行者 </summary>
    public interface IMicroExecutor
    {
        /// <summary> 执行 </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Execute(IMessageSender sender, InvokeMessage message);
    }
}
