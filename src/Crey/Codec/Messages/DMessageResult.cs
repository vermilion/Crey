using MessagePack;

namespace Crey.Codec.Messages;

[MessagePackObject(keyAsPropertyName: true)]
public class DMessageResult : TransportMessage<MessageResult>
{
    public string Id { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public DMessageDynamic Content { get; set; }

    public override void SetValue(MessageResult message, IMessageSerializer serializer)
    {
        Id = message.Id;
        IsSuccess = message.IsSuccess;
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
            IsSuccess = IsSuccess,
            Message = Message,
        };

        if (Content != null)
        {
            result.Content = Content.GetValue(serializer);
        }

        return result;
    }
}
