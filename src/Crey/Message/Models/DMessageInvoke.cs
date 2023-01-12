using MessagePack;

namespace Crey.Message;

[MessagePackObject(keyAsPropertyName: true)]
public class DMessageInvoke : Message
{
    public string ServiceId { get; set; }
    public IDictionary<string, DMessageDynamic> Parameters { get; set; }
    public DMessageInvokeContextDynamic Context { get; set; }

    public void SetValue(InvokeMessage message, IMessageSerializer serializer)
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

    public InvokeMessage GetValue(IMessageSerializer serializer)
    {
        var message = new InvokeMessage
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
