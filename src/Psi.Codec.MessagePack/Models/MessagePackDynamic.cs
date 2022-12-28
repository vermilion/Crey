using MessagePack;
using Psi.Message.Models;

namespace Psi.Codec.MessagePack.Models;

[MessagePackObject(keyAsPropertyName: true)]
public class MessagePackDynamic : DMessageDynamic
{
    public MessagePackDynamic() : base(new MessagePackMessageSerializer()) { }
}
