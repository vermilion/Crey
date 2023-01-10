namespace Crey.Message;

public interface IMessageDecoder
{
    Task<object> DecodeAsync(byte[] data, Type type);
}
