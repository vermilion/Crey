using MessagePack;
using Psi.Message.Models;

namespace Psi.Codec.MessagePack.Models;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackResult : DMessageResult<MessagePackDynamic>
{
    public MessagePackResult() { }

    public MessagePackResult(MessageResult message) : base(message)
    {
    }
}
