using MessagePack;
using Crey.Message;

namespace Crey.Codec.MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackResult : DMessageResult<MessagePackDynamic>
{
    public MessagePackResult() { }

    public MessagePackResult(MessageResult message) : base(message)
    {
    }
}
