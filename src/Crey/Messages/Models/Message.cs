namespace Crey.Messages;

public abstract class Message
{
    public virtual string Id { get; set; }

    public Message(string id = null)
    {
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString("N") : id;
    }
}
