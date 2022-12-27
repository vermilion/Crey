using System;

namespace Spear.Core.Message.Abstractions;

public interface IMessageSerializer
{
    byte[] Serialize(object value);

    byte[] SerializeNoType(object value);

    object Deserialize(byte[] data, Type type);

    object DeserializeNoType(byte[] data, Type type);

    T Deserialize<T>(byte[] data);
}
