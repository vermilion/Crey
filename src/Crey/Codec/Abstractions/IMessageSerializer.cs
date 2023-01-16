namespace Crey.Codec;

public interface IMessageSerializer
{
    byte[] Serialize(object value);

    object Deserialize(byte[] data, Type type);

    T Deserialize<T>(byte[] data);
}
