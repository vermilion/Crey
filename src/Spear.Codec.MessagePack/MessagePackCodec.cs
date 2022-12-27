using Spear.Codec.MessagePack.Models;
using Spear.Core.Message;
using Spear.Core.Message.Abstractions;

namespace Spear.Codec.MessagePack;

public class MessagePackCodec : MessageCodec<MessagePackDynamic, MessagePackInvoke, MessagePackResult>
{
    public MessagePackCodec(IMessageSerializer serializer) 
        : base(serializer)
    {
    }
}
