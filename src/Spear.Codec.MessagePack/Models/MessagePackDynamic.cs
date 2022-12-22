using MessagePack;
using Spear.Core.Message.Models;

namespace Spear.Codec.MessagePack.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class MessagePackDynamic : DMessageDynamic
    {
        public MessagePackDynamic() : base(new MessagePackMessageSerializer()) { }
    }
}
