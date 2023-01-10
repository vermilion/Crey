using Crey.Codec.MessagePack;
using Crey.Message;

namespace Crey.Codec.MessagePack;

internal class MessagePackCodec : MessageCodec<MessagePackDynamic, MessagePackInvoke, MessagePackResult>
{
    public MessagePackCodec(IMessageSerializer serializer)
        : base(serializer)
    {
    }
}
