using MessagePack;
using Psi.Message.Models;

namespace Psi.Codec.MessagePack.Models;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackInvoke : DMessageInvoke<MessagePackDynamic>
{
    public MessagePackInvoke() { }

    public MessagePackInvoke(InvokeMessage message) : base(message)
    {
    }
}
