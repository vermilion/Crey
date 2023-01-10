using Crey.Message;
using MessagePack;
using MessagePack.Resolvers;

namespace Crey.Codec.MessagePack;

internal class MessagePackMessageSerializer : IMessageSerializer
{
    public byte[] Serialize(object value)
    {
        if (value == null) return new byte[0];

        return MessagePackSerializer.Serialize(value);
    }

    public byte[] SerializeNoType(object value)
    {
        if (value == null) return new byte[0];

        return MessagePackSerializer.Serialize(value, ContractlessStandardResolver.Options);
    }

    public object Deserialize(byte[] data, Type type)
    {
        return data == null ? null : MessagePackSerializer.Deserialize(type, data);
    }

    public object DeserializeNoType(byte[] data, Type type)
    {
        return data == null
            ? null
            : MessagePackSerializer.Deserialize(type, data, ContractlessStandardResolver.Options);
    }

    public T Deserialize<T>(byte[] data)
    {
        return data == null ? default : MessagePackSerializer.Deserialize<T>(data);
    }
}
