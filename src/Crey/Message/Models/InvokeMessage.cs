namespace Crey.Message;

public class InvokeMessage : DMessage
{
    public string ServiceId { get; set; }
    public IDictionary<string, object> Parameters { get; set; }
    public IDictionary<string, string?> Headers { get; set; }

    public InvokeMessage()
    {
        Parameters = new Dictionary<string, object>();
    }
}
