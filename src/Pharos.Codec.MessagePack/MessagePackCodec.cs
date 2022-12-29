using Psi.Codec.MessagePack.Models;
using Psi.Message;
using Psi.Message.Abstractions;

namespace Psi.Codec.MessagePack;

public class MessagePackCodec : MessageCodec<MessagePackDynamic, MessagePackInvoke, MessagePackResult>
{
    public MessagePackCodec(IMessageSerializer serializer)
        : base(serializer)
    {
    }
}
