namespace Crey.Message;

public interface IMessageEncoder
{
    Task<byte[]> EncodeAsync(object message);
}
