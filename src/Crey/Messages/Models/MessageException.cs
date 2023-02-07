namespace Crey.Messages;

public class MessageException
{
    public string ExceptionTypeName { get; set; }
    public string Source { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }

    public MessageException()
    {
    }

    public MessageException(Exception ex)
    {
        ExceptionTypeName = ex.GetType().FullName;
        Source = ex.Source;
        Message = ex.Message;
        StackTrace = ex.StackTrace;
    }
}

