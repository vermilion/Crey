using MessagePack;
using Crey.Message.Models;

namespace Crey.Codec.MessagePack.Models;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackDynamic : DMessageDynamic
{
    public MessagePackDynamic() : base(new MessagePackMessageSerializer()) { }
}
