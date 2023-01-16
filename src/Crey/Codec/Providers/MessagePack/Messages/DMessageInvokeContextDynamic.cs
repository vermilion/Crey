using Crey.Helper;
using MessagePack;

namespace Crey.Codec.MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class DMessageInvokeContextDynamic : TransportMessage<MessageInvokeContext>
{
    public byte[] Content { get; set; }

    public override void SetValue(MessageInvokeContext context, IMessageSerializer serializer)
    {
        Content = serializer.Serialize(JsonHelper.ToJson(context));
    }

    public override MessageInvokeContext GetValue(IMessageSerializer serializer)
    {
        var content = serializer.Deserialize<string>(Content);
        return JsonHelper.FromJson<MessageInvokeContext>(content);
    }
}
