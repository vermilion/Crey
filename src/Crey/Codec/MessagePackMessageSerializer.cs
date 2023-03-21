using MessagePack;

namespace Crey.Codec;

public class MessagePackMessageSerializer : IMessageSerializer
{
    public byte[] Serialize(object value)
    {
        if (value == null) return new byte[0];

        return MessagePackSerializer.Serialize(value);
    }

    public object Deserialize(byte[] data, Type type)
    {
        return data == null ? null : MessagePackSerializer.Deserialize(type, data);
    }

    public T Deserialize<T>(byte[] data)
    {
        return data == null ? default : MessagePackSerializer.Deserialize<T>(data);
    }
}
