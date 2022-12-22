using System.Threading.Tasks;
using Spear.Core.Message;
using Spear.Core.Message.Models;

namespace Spear.Protocol.Grpc.Sender
{
    public class GrpcServerMessageSender : IMessageSender
    {
        private readonly TaskCompletionSource<MessageResult> _completion;

        public GrpcServerMessageSender(TaskCompletionSource<MessageResult> completion)
        {
            _completion = completion;
        }

        public Task Send(DMessage message, bool flush = true)
        {
            if (message is not MessageResult result)
            {
                _completion.SetCanceled();
                return Task.CompletedTask;
            }

            _completion.SetResult(result);
            return Task.CompletedTask;
        }
    }
}
