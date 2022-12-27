using MessagePack;
using Spear.Core.Message.Models;

namespace Spear.Codec.MessagePack.Models;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackInvoke : DMessageInvoke<MessagePackDynamic>
{
    public MessagePackInvoke() { }

    public MessagePackInvoke(InvokeMessage message) : base(message)
    {
    }
}
