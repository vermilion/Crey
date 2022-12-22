using MessagePack;
using Spear.Core.Message.Models;

namespace Spear.Codec.MessagePack.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class MessagePackResult : DMessageResult<MessagePackDynamic>
    {
        public MessagePackResult() { }

        public MessagePackResult(MessageResult message) : base(message)
        {
        }
    }
}
