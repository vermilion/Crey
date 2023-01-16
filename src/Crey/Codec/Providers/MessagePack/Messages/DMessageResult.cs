using MessagePack;

namespace Crey.Codec.MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class DMessageResult : TransportMessageResult
{
    public string Id { get; set; }
    public int Code { get; set; }
    public string Message { get; set; }
    public DMessageDynamic Content { get; set; }

    public override void SetValue(MessageResult message, IMessageSerializer serializer)
    {
        Id = message.Id;
        Code = message.Code;
        Message = message.Message;

        if (message.Content != null)
        {
            Content = new DMessageDynamic();
            Content.SetValue(message.Content, serializer);
        }
    }

    public override MessageResult GetValue(IMessageSerializer serializer)
    {
        var result = new MessageResult
        {
            Id = Id,
            Code = Code,
            Message = Message,
        };

        if (Content != null)
        {
            result.Content = Content.GetValue(serializer);
        }

        return result;
    }
}
