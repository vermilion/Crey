using MessagePack;

namespace Crey.Codec.Messages;

[MessagePackObject(keyAsPropertyName: true)]
public class DMessageInvoke : TransportMessage<MessageInvoke>
{
    public string Id { get; set; }
    public string ServiceId { get; set; }
    public IDictionary<string, DMessageDynamic> Parameters { get; set; }
    public DMessageInvokeContextDynamic Context { get; set; }

    public override void SetValue(MessageInvoke message, IMessageSerializer serializer)
    {
        Id = message.Id;
        ServiceId = message.ServiceId;

        if (message.Parameters is not null)
        {
            Parameters = message.Parameters.ToDictionary(k => k.Key, v =>
            {
                var item = new DMessageDynamic();
                item.SetValue(v.Value, serializer);
                return item;
            });
        }

        Context = new DMessageInvokeContextDynamic();
        Context.SetValue(message.Context, serializer);
    }

    public override MessageInvoke GetValue(IMessageSerializer serializer)
    {
        var message = new MessageInvoke
        {
            Id = Id,
            ServiceId = ServiceId
        };

        if (Parameters is not null)
        {
            message.Parameters = Parameters.ToDictionary(k => k.Key, v => v.Value.GetValue(serializer));
        }

        if (Context is not null)
        {
            message.Context = Context.GetValue(serializer);
        }

        return message;
    }
}
