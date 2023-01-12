using Crey.Extensions;

namespace Crey.Message;

internal class MessageCodec : IMessageCodec
{
    private readonly IMessageSerializer _serializer;

    public MessageCodec(IMessageSerializer serializer)
    {
        _serializer = serializer;
    }

    protected virtual byte[] OnEncode(object message)
    {
        if (message == null) return new byte[0];

        if (message.GetType() == typeof(byte[]))
            return (byte[])message;

        if (message is InvokeMessage invoke)
        {
            var model = new DMessageInvoke();
            model.SetValue(invoke, _serializer);
            return _serializer.Serialize(model);
        }

        if (message is MessageResult result)
        {
            var model = new DMessageResult();
            model.SetValue(result, _serializer);
            return _serializer.Serialize(model);
        }

        return _serializer.SerializeNoType(message);
    }

    protected virtual object OnDecode(byte[] data, Type type)
    {
        if (data == null || data.Length == 0)
            return null;

        if (type == typeof(InvokeMessage))
        {
            var item = _serializer.Deserialize<DMessageInvoke>(data);
            return item.GetValue(_serializer);
        }

        if (type == typeof(MessageResult))
        {
            var item = _serializer.Deserialize<DMessageResult>(data);
            return item.GetValue(_serializer);
        }

        return _serializer.DeserializeNoType(data, type);
    }

    public async Task<byte[]> EncodeAsync(object message)
    {
        if (message == null) return new byte[0];
        var buffer = OnEncode(message);
        return buffer;
    }

    public async Task<object> DecodeAsync(byte[] data, Type type)
    {
        var obj = OnDecode(data, type);
        var result = obj.CastTo(type);
        return result;
    }
}
