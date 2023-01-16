namespace Crey.Codec;

public interface IMessageCodec
{
    Task<byte[]> EncodeAsync(object message);
    Task<object> DecodeAsync(byte[] data, Type type);
}
