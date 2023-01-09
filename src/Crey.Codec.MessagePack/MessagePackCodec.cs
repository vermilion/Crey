using Crey.Codec.MessagePack.Models;
using Crey.Message;
using Crey.Message.Abstractions;

namespace Crey.Codec.MessagePack;

internal class MessagePackCodec : MessageCodec<MessagePackDynamic, MessagePackInvoke, MessagePackResult>
{
    public MessagePackCodec(IMessageSerializer serializer)
        : base(serializer)
    {
    }
}
