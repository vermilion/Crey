using MessagePack;
using Crey.Message.Models;

namespace Crey.Codec.MessagePack.Models;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackInvoke : DMessageInvoke<MessagePackDynamic>
{
    public MessagePackInvoke() { }

    public MessagePackInvoke(InvokeMessage message) : base(message)
    {
    }
}
