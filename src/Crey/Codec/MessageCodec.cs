using Crey.Extensions;

namespace Crey.Codec;

public abstract class MessageCodec<TMessageInvoke, TMessageResult> : IMessageCodec
    where TMessageInvoke : TransportMessageInvoke, new()
    where TMessageResult : TransportMessageResult, new()
{
    private readonly IMessageSerializer _serializer;

    public MessageCodec(IMessageSerializer serializer)
    {
        _serializer = serializer;
    }

    public async Task<byte[]> EncodeAsync(object message)
    {
        if (message == null) 
            return new byte[0];

        switch (message)
        {
            case MessageInvoke invokeMessage:
                {
                    var model = new TMessageInvoke();
                    model.SetValue(invokeMessage, _serializer);
                    return _serializer.Serialize(model);
                }
            case MessageResult resultMessage:
                {
                    var model = new TMessageResult();
                    model.SetValue(resultMessage, _serializer);
                    return _serializer.Serialize(model);
                }
            default:
                throw new InvalidOperationException($"Message type {message.GetType()} is not supported");
        }
    }

    public async Task<object> DecodeAsync(byte[] data, Type type)
    {
        if (data == null || data.Length == 0)
            return null;

        if (type == typeof(MessageInvoke))
        {
            var item = _serializer.Deserialize<TMessageInvoke>(data);
            return item.GetValue(_serializer).CastTo(type);
        }

        if (type == typeof(MessageResult))
        {
            var item = _serializer.Deserialize<TMessageResult>(data);
            return item.GetValue(_serializer).CastTo(type);
        }

        throw new InvalidOperationException($"Message type {type} is not supported");
    }
}
