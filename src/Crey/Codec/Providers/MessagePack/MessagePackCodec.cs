namespace Crey.Codec.MessagePack;

public class MessagePackCodec : MessageCodec<DMessageInvoke, DMessageResult>
{
    public MessagePackCodec(IMessageSerializer serializer)
        : base(serializer)
    {
    }
}
