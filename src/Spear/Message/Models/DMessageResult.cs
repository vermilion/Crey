namespace Spear.Core.Message.Models;

public class DMessageResult<TDynamic>
    where TDynamic : DMessageDynamic, new()
{
    public virtual string Id { get; set; }
    public virtual int Code { get; set; }
    public virtual string Message { get; set; }
    public virtual TDynamic Content { get; set; }

    public DMessageResult() { }

    public DMessageResult(MessageResult message)
    {
        SetResult(message);
    }

    public void SetResult(MessageResult message)
    {
        Id = message.Id;
        Code = message.Code;
        Message = message.Message;
        if (message.Content != null)
        {
            Content = new TDynamic();
            Content.SetValue(message.Content);
        }
    }

    public MessageResult GetValue()
    {
        var result = new MessageResult
        {
            Id = Id,
            Code = Code,
            Message = Message,
        };

        if (Content != null)
        {
            result.Content = Content.GetValue();
        }

        return result;
    }
}
