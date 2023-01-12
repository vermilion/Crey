using Crey.Extensions;
using Crey.Helper;
using MessagePack;

namespace Crey.Message;

[MessagePackObject(keyAsPropertyName: true)]
public class DMessageDynamic
{
    public string ContentType { get; set; }
    public byte[] Content { get; set; }

    public void SetValue(object value, IMessageSerializer serializer)
    {
        if (value == null)
            return;

        var type = value.GetType();
        ContentType = type.TypeName();
        var code = Type.GetTypeCode(type);
        Content = serializer.Serialize(code == TypeCode.Object ? JsonHelper.ToJson(value) : value);
    }

    public object GetValue(IMessageSerializer serializer)
    {
        if (Content == null || string.IsNullOrWhiteSpace(ContentType))
            return null;

        var type = Type.GetType(ContentType);
        var code = Type.GetTypeCode(type);
        if (code == TypeCode.Object)
        {
            var content = serializer.Deserialize<string>(Content);
            return JsonHelper.FromJson(content, type);
        }

        return serializer.Deserialize(Content, type);
    }
}
