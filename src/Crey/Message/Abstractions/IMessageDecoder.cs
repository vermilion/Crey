namespace Crey.Message.Abstractions;

public interface IMessageDecoder
{
    Task<object> DecodeAsync(byte[] data, Type type);
}
