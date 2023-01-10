using MessagePack;
using Crey.Message;

namespace Crey.Codec.MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackInvoke : DMessageInvoke<MessagePackDynamic>
{
    public MessagePackInvoke() { }

    public MessagePackInvoke(InvokeMessage message) : base(message)
    {
    }
}
