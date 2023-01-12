using Crey.Helper;
using MessagePack;

namespace Crey.Message;

[MessagePackObject(keyAsPropertyName: true)]
public class DMessageInvokeContextDynamic
{
    public byte[] Content { get; set; }

    public void SetValue(InvokeMethodContext context, IMessageSerializer serializer)
    {
        Content = serializer.Serialize(JsonHelper.ToJson(context));
    }

    public InvokeMethodContext GetValue(IMessageSerializer serializer)
    {
        var content = serializer.Deserialize<string>(Content);
        return JsonHelper.FromJson<InvokeMethodContext>(content);
    }
}
