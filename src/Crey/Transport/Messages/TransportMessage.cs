namespace Crey.Transport;

public abstract class TransportMessage<TMessage>
    where TMessage : class
{
    public abstract TMessage GetValue(IMessageSerializer serializer);
    public abstract void SetValue(TMessage message, IMessageSerializer serializer);
}
