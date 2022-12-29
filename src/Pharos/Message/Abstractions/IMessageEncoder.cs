namespace Psi.Message.Abstractions;

public interface IMessageEncoder
{
    Task<byte[]> EncodeAsync(object message);
}
