namespace Spear.Core.Message.Abstractions;

public interface IMessageEncoder
{
    Task<byte[]> EncodeAsync(object message);
}
