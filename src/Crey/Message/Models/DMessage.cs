namespace Crey.Message.Models;

public class DMessage
{
    public virtual string Id { get; set; }

    public DMessage(string id = null)
    {
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString("N") : id;
    }
}
