using Crey.Codec.Messages;
using Crey.Extensions;

namespace Crey.Codec;

public class MessagePackCodec : IMessageCodec
{
    private readonly IMessageSerializer _serializer;

    public MessagePackCodec(IMessageSerializer serializer)
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
                    var model = new DMessageInvoke();
                    model.SetValue(invokeMessage, _serializer);
                    return _serializer.Serialize(model);
                }
            case MessageResult resultMessage:
                {
                    var model = new DMessageResult();
                    model.SetValue(resultMessage, _serializer);
                    return _serializer.Serialize(model);
                }
            default:
                throw new InvalidOperationException($"Message type {message.GetType()} is not supported");
        }
    }

    public async Task<object?> DecodeAsync(byte[] data, Type type)
    {
        if (data == null || data.Length == 0)
            return null;

        if (type == typeof(MessageInvoke))
        {
            var item = _serializer.Deserialize<DMessageInvoke>(data);
            return item.GetValue(_serializer).CastTo(type);
        }

        if (type == typeof(MessageResult))
        {
            var item = _serializer.Deserialize<DMessageResult>(data);
            return item.GetValue(_serializer).CastTo(type);
        }

        throw new InvalidOperationException($"Message type {type} is not supported");
    }
}
