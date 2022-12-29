using MessagePack;
using Crey.Message.Models;

namespace Crey.Codec.MessagePack.Models;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackResult : DMessageResult<MessagePackDynamic>
{
    public MessagePackResult() { }

    public MessagePackResult(MessageResult message) : base(message)
    {
    }
}
