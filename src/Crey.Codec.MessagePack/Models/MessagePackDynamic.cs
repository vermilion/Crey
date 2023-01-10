using MessagePack;
using Crey.Message;

namespace Crey.Codec.MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackDynamic : DMessageDynamic
{
    public MessagePackDynamic() : base(new MessagePackMessageSerializer()) { }
}
