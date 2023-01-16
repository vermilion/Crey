using System;
using System.Collections.Generic;
using System.Text;

namespace Crey.Codec.MessagePack;

public class MessagePackCodec : MessageCodec<DMessageInvoke, DMessageResult>
{
    public MessagePackCodec(IMessageSerializer serializer)
        : base(serializer)
    {
    }
}
